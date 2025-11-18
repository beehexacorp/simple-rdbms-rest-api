using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Dapper;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;
using System.Text;
using System.Text.Json;
using System.Dynamic;

namespace SimpleRDBMSRestfulAPI.Helpers;

public class SqlServerDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public SqlServerDataHelper(IAppSettings appSettings)
        : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.SQL_SERVER;

    public override (string query, IDictionary<string, object> parameters) BuildQuery(EntityRequestMetadata request)
    {
        var parameters = request.@params ?? new Dictionary<string, object>();
        var fields = (request.Fields != null && request.Fields.Any())
            ? string.Join(", ", request.Fields)
            : "*";
        var conditions = (request.Conditions != null && request.Conditions.Any())
            ? string.Join(" AND ", request.Conditions)
            : "";
        conditions = !string.IsNullOrWhiteSpace(conditions) ? $"WHERE {conditions}" : "";

        // MSSQL uses TOP for limiting results
        string top = parameters.ContainsKey("limit") ? $"TOP {parameters["limit"]}" : "";

        // Order by clause
        var orderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? $"ORDER BY {request.OrderBy}"
            : "";

        // Build query
        var query = $@"SELECT {top} {fields}
FROM {request.TableName}
{conditions}
{orderBy}";

        // Remove limit from parameters since it's used in TOP
        parameters.Remove("limit");

        return (query: query.Trim(), parameters: parameters);
    }

    public override async Task ConnectAsync(string connectionString)
    {
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var _ = await conn.QueryFirstOrDefaultAsync<bool?>(@"SELECT TOP 1 1
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_TYPE = 'BASE TABLE'
        ORDER BY TABLE_NAME;");
    }

    public override System.Data.IDbConnection CreateConnection(string? connectionString = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var builder = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        return builder;
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.InitialCatalog;
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {
        var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return ParseHost(builder.DataSource);
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return ParsePort(builder.DataSource);
    }

    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.UserID;
    }

    // Helper method to parse host from DataSource
    static string ParseHost(string dataSource)
    {
        if (dataSource.Contains(","))
        {
            return dataSource.Split(',')[0];
        }
        return dataSource;
    }

    // Helper method to parse port from DataSource
    static string ParsePort(string dataSource)
    {
        if (dataSource.Contains(","))
        {
            return dataSource.Split(',')[1];
        }
        return "1433"; // Default SQL Server port
    }

    public override async Task<CursorBasedResult> GetTables(
    Settings.ConnectionInfoDTO connectionInfo,
    string? query,
    CursorDirection rel,
    string? cursor,
    int limit,
    int offset)
    {
        var connectionString = Convert.FromBase64String(connectionInfo.ConnectionString).DecryptAES();
        using var conn = CreateConnection(connectionString);
        conn.Open();

        var sql = @"
        SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_TYPE = 'BASE TABLE'
        " + (string.IsNullOrWhiteSpace(query) ? "" : "AND TABLE_NAME LIKE @Query") + @"
        ORDER BY TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME
        OFFSET @Offset ROWS 
        FETCH NEXT (@Limit) ROWS ONLY";

        var parameters = new
        {
            Limit = limit,
            Offset = offset,
            Query = "%" + query + "%"
        };

        var resp = await conn.QueryAsync(sql, parameters);
        var results = resp.Cast<IDictionary<string, object>>().ToList();

        return new CursorBasedResult
        {
            FirstCursor = results.FirstOrDefault()?["TABLE_NAME"]?.ToString(),
            LastCursor = results.LastOrDefault()?["TABLE_NAME"]?.ToString(),
            Items = results
        };
    }

    public override async Task<IEnumerable<IDictionary<string, object>>> GetTableFields(
    Settings.ConnectionInfoDTO connectionInfo,
    IDictionary<string, object>? data)
    {
        if (data == null || !data.ContainsKey("table_catalog") || !data.ContainsKey("table_schema") || !data.ContainsKey("table_name"))
            throw new Exception("Should provide table_catalog, table_schema and table_name");

        var tableCatalog = data["table_catalog"].ToString();
        var tableSchema = data["table_schema"].ToString();
        var tableName = data["table_name"].ToString();

        var encryptedBytes = Convert.FromBase64String(connectionInfo.ConnectionString);
        var connectionString = encryptedBytes.DecryptAES();
        using var conn = CreateConnection(connectionString);
        conn.Open();

        var sql = @"
        SELECT 
            COLUMN_NAME,
            DATA_TYPE,
            IS_NULLABLE,
            CHARACTER_MAXIMUM_LENGTH,
            NUMERIC_PRECISION,
            NUMERIC_SCALE,
            COLUMN_DEFAULT
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE 
            TABLE_CATALOG = @TableCatalog AND 
            TABLE_SCHEMA = @TableSchema AND 
            TABLE_NAME = @TableName
        ORDER BY ORDINAL_POSITION;";

        var resp = await conn.QueryAsync(sql, new
        {
            TableCatalog = tableCatalog,
            TableSchema = tableSchema,
            TableName = tableName
        });

        return resp
    .Select(row => (IDictionary<string, object?>)row)
    .ToList();
    }

    // CRUD Operations
    public override async Task<IDictionary<string, object>> CreateRecord(
        string connectionString,
        string tableName,
        IDictionary<string, object> data)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentNullException(nameof(tableName));
        if (data == null || data.Count == 0)
            throw new ArgumentException("Data dictionary cannot be empty.", nameof(data));

        var keys = data.Keys.ToList();
        var columns = string.Join(", ", keys.Select(k => $"[{k}]"));
        var paramNames = string.Join(", ", keys.Select(k => "@" + k));

        var parameters = PrepareParams(data);
        var query = $@"
            INSERT INTO [{tableName}] ({columns})
            OUTPUT inserted.*
            VALUES ({paramNames});
        ";

        var decryptedConnectionString = Convert.FromBase64String(connectionString).DecryptAES();

        using var conn = CreateConnection(decryptedConnectionString);
        var insertedRecord = await conn.QuerySingleOrDefaultAsync(query, (object)parameters);
        return insertedRecord ?? new Dictionary<string, object>();
    }

    public override async Task<IEnumerable<IDictionary<string, object>>> ReadRecord(
        string connectionStringBase64,
        string tableName,
        IDictionary<string, object>? filters = null)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentNullException(nameof(tableName));

        // Add WHERE clause if filters exist
        DynamicParameters parameters = new DynamicParameters();

        var whereClause = BuildWhereClause(filters, parameters);

        var decryptedConnectionString = Convert.FromBase64String(connectionStringBase64).DecryptAES();
        using var conn = new Microsoft.Data.SqlClient.SqlConnection(decryptedConnectionString);
        await conn.OpenAsync();
        
        var query = new StringBuilder($"SELECT * FROM [{tableName}] {whereClause}");

        var result = await conn.QueryAsync<dynamic>(query.ToString(), parameters);

        // Convert Dapper dynamic rows to IDictionary<string, object>
        return result.Select(r => (IDictionary<string, object>)r).ToList();
    }

    public override async Task<IDictionary<string, object>> UpdateRecord(
        string connectionString,
        string tableName,
        IDictionary<string, object> data,
        IDictionary<string, object> filters)
    {
        if (data == null || data.Count == 0)
            throw new ArgumentException("Data dictionary cannot be empty.", nameof(data));
        if (filters == null || filters.Count == 0)
        throw new ArgumentException("Filters cannot be empty for DELETE.", nameof(filters));

        var parameters = PrepareParams(data);
        var whereClause = BuildWhereClause(filters, parameters);

        var setClause = string.Join(", ", data.Keys.Select(k => $"[{k}] = @{k}"));
        var query = $@"
        UPDATE [{tableName}]
        SET {setClause}
        OUTPUT INSERTED.*
        {whereClause};
    ";

        var decryptedConnectionString = Convert.FromBase64String(connectionString).DecryptAES();
        using var conn = CreateConnection(decryptedConnectionString);
        var updatedRecord = await conn.QuerySingleOrDefaultAsync(query, (object)parameters);
        return updatedRecord ?? new Dictionary<string, object>();
    }


    public override async Task<IDictionary<string, object>> DeleteRecord(
        string connectionString,
        string tableName,
        IDictionary<string, object> filters)
    {

        if (filters == null || filters.Count == 0)
        throw new ArgumentException("Filters cannot be empty for DELETE.", nameof(filters));

        var parameters = new DynamicParameters();
        var whereClause = BuildWhereClause(filters, parameters);

        var query = $@"
        DELETE FROM [{tableName}]
        OUTPUT DELETED.*
        {whereClause};
    ";

        var decryptedConnectionString = Convert.FromBase64String(connectionString).DecryptAES();
        using var conn = CreateConnection(decryptedConnectionString);
        var deletedRecord = await conn.QuerySingleOrDefaultAsync(query, parameters);
        return deletedRecord ?? new Dictionary<string, object>();
    }

    private static DynamicParameters PrepareParams(IDictionary<string, object> data)
    {
        var parameters = new DynamicParameters();

        foreach (var kvp in data)
        {
            object value = kvp.Value;

            if (value is JsonElement e)
            {
                value = e.ValueKind switch
                {
                    JsonValueKind.String => e.GetString(),
                    JsonValueKind.Number => e.TryGetInt64(out var l) ? l : e.TryGetDouble(out var d) ? d : 0,
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null or JsonValueKind.Undefined => null,
                    _ => e.GetRawText()
                };
            }

            parameters.Add(kvp.Key, value ?? DBNull.Value);
        }

        return parameters;
    }

    private static string BuildWhereClause(
    IDictionary<string, object> filters,
    DynamicParameters dynamicParams)
    {
        if (filters == null || filters.Count == 0)
            return string.Empty;

        var whereClauses = new List<string>();

        foreach (var kv in filters)
        {
            var name = kv.Key;

            if (kv.Value == null)
            {
                whereClauses.Add($"[{name}] IS NULL");
            }
            else
            {
                whereClauses.Add($"[{name}] = @{name}");
                dynamicParams.Add($"@{name}", kv.Value);
            }
        }

        return " WHERE " + string.Join(" AND ", whereClauses);
    }
}