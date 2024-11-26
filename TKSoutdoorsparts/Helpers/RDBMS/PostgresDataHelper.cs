using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public class PostgresDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public PostgresDataHelper(IAppSettings appSettings)
        : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.POSTGRES;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        Dictionary<string, object> @params = request.@params;
        @params = @params ?? new Dictionary<string, object>();
        if (!@params.ContainsKey("limit"))
        {
            throw new ArgumentNullException("The @limit param is required");
        }
        if (!@params.ContainsKey("offset"))
        {
            throw new ArgumentNullException("The @offset param is required");
        }
        var fields =
            request.Fields != null && request.Fields.Any()
                ? string.Join(", ", request.Fields)
                : "*";
        var conditions =
            request.Conditions != null && request.Conditions.Any()
                ? string.Join("AND ", request.Conditions)
                : "";
        conditions = !string.IsNullOrWhiteSpace(conditions) ? $"where {conditions}" : "";
        request.OrderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? $"order by {request.OrderBy}"
            : "";
        var query =
            $@"SELECT {fields} 
FROM {request.TableName} 
{conditions} 
{request.OrderBy} 
LIMIT @limit 
OFFSET @offset";
        return query.Trim();
    }

    public override async Task ConnectAsync(string connectionString)
    {
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var _ = await conn.QueryFirstOrDefaultAsync<bool?>(@"SELECT 1
FROM information_schema.tables
WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
  AND table_type = 'BASE TABLE'
LIMIT 1;");
    }


    public override IDbConnection CreateConnection(string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        return new NpgsqlConnection(connectionString);
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.Database ?? "N/A";
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.Host ?? "N/A";
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.Port.ToString();
    }

    public override async Task<IEnumerable<IDictionary<string, object>>> GetTableFields(
        Settings.ConnectionInfoDTO connectonInfo,
        IDictionary<string, object>? data)
    {
        if (data == null || !data.ContainsKey("table_catalog") || !data.ContainsKey("table_schema") || !data.ContainsKey("table_name"))
        {
            throw new Exception("Should provide table_catalog, table_schema and table_name");
        }
        var tableCatalog = data["table_catalog"].ToString();
        var tableSchema = data["table_schema"].ToString();
        var tableName = data["table_name"].ToString();

        var connectionString = Convert.FromBase64String(connectonInfo.ConnectionString).DecryptAES();
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var sql = @"SELECT 
    column_name,
    data_type,
    is_nullable,
    character_maximum_length,
    numeric_precision,
    numeric_scale,
    column_default
FROM 
    information_schema.columns
WHERE 
    table_catalog = @tableCatalog AND 
    table_schema = @tableSchema AND 
    table_name = @tableName
ORDER BY 
    ordinal_position;";
        var resp = await conn.QueryAsync(sql, new
        {
            tableCatalog,
            tableSchema,
            tableName
        });
        var results = resp.Cast<IDictionary<string, object>>().ToList();
        return results;
    }


    public override async Task<CursorBasedResult> GetTables(
        Settings.ConnectionInfoDTO connectonInfo,
        string? query,
        CursorDirection rel,
        string? cursor,
        int limit,
        int offset)
    {
        var sql = @"WITH filtered_tables AS (
    SELECT 
        table_catalog,
        table_schema,
        table_name,
        to_jsonb(jsonb_build_object(
            'table_catalog', table_catalog,
            'table_schema', table_schema,
            'table_name', table_name
        )) AS cursor_data,
        ts_rank_cd(
            to_tsvector('simple', table_name),
            phraseto_tsquery('simple', @query)
        ) AS rank
    FROM information_schema.tables
    WHERE table_schema NOT IN ('pg_catalog', 'information_schema')
      AND table_type = 'BASE TABLE'
      AND (
          (@query IS NULL) OR
          (to_tsvector('simple', table_name) @@ phraseto_tsquery('simple', @query)) OR
          (similarity(table_name::text, @query) > 0.2) OR
          (table_name ~* @query)
      )
),
cursor_decoded AS (
    SELECT
        (jsonb_each_text(convert_from(decode(@cursor, 'base64'), 'UTF8')::jsonb)).*
),
decoded_values AS (
    SELECT
        MAX(CASE WHEN key = 'table_catalog' THEN value END) AS table_catalog,
        MAX(CASE WHEN key = 'table_schema' THEN value END) AS table_schema,
        MAX(CASE WHEN key = 'table_name' THEN value END) AS table_name
    FROM cursor_decoded
),
encoded_cursor AS (
    SELECT
        table_catalog,
        table_schema,
        table_name,
        encode(to_jsonb(cursor_data)::text::bytea, 'base64') AS __cursor,
        rank
    FROM filtered_tables
),
paged_query AS (
    SELECT
        *
    FROM encoded_cursor
    WHERE (
        @cursor IS NULL OR -- No cursor provided
        (
            @cursor IS NOT NULL AND @rel = 1 AND
            (table_catalog, table_schema, table_name) >
            (SELECT table_catalog, table_schema, table_name FROM decoded_values)
        )
        OR
        (
            @cursor IS NOT NULL AND @rel = 0 AND
            (table_catalog, table_schema, table_name) <
            (SELECT table_catalog, table_schema, table_name FROM decoded_values)
        )
    )
)
SELECT
    table_catalog,
    table_schema,
    table_name,
    __cursor
FROM paged_query
ORDER BY
    table_catalog,
    table_schema,
    table_name
LIMIT @limit
OFFSET @offset;
";
        var connectionString = Convert.FromBase64String(connectonInfo.ConnectionString).DecryptAES();
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var resp = await conn.QueryAsync(sql, new
        {
            query = query,
            cursor = cursor,
            rel = (int)rel,
            limit = limit,
            offset = offset
        });
        var results = resp.Cast<IDictionary<string, object>>().ToList();
        return new CursorBasedResult
        {
            FirstCursor = results.FirstOrDefault()?["__cursor"]?.ToString(),
            LastCursor = results.LastOrDefault()?["__cursor"]?.ToString(),
            Items = results
        };
    }


    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.Username ?? "N/A";
    }

}
