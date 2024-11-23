using System.Data;
using Dapper;
using Npgsql;
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
        return new NpgsqlConnection(!string.IsNullOrWhiteSpace(connectionString) ? connectionString : _appSettings.GetConnectionString());
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

    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.Username ?? "N/A";
    }

}
