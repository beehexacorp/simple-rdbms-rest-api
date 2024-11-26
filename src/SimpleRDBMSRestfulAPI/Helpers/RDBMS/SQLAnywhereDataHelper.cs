using System.Data.Odbc;
using Dapper;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public class SqlAnywhereDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public SqlAnywhereDataHelper(IAppSettings appSettings)
        : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.SQL_ANYWHERE;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        Dictionary<string, object> @params = request.@params ?? new Dictionary<string, object>();
        if (!@params.ContainsKey("top"))
        {
            throw new ArgumentNullException("The @top param is required");
        }

        if (!@params.ContainsKey("startAt"))
        {
            throw new ArgumentNullException("The @startAt param is required");
        }
        var topValue = @params["top"];
        var startAtValue = @params["startAt"];
        var fields =
            request.Fields != null && request.Fields.Any()
                ? string.Join(", ", request.Fields)
                : "*";
        var conditions =
            request.Conditions != null && request.Conditions.Any()
                ? string.Join("AND ", request.Conditions)
                : "";
        conditions = !string.IsNullOrWhiteSpace(conditions) ? $"WHERE {conditions}" : "";
        request.OrderBy = !string.IsNullOrWhiteSpace(request.OrderBy)
            ? $"ORDER BY {request.OrderBy}"
            : "";
        var query =
            $@"SELECT TOP {topValue} START AT {startAtValue} {fields}
FROM {request.TableName}
{conditions}
{request.OrderBy}";
        return query.Trim();
    }

    public override async Task ConnectAsync(string connectionString)
    {
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var _ = await conn.QueryFirstOrDefaultAsync<bool?>(@"SELECT 1
FROM SYS.SYSTABLE
WHERE creator NOT IN ('SYS', 'dbo')
  AND table_type = 'BASE';");
    }

    public override System.Data.IDbConnection CreateConnection(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        var builder = new OdbcConnectionStringBuilder(connectionString);
        return new OdbcConnection(builder.ConnectionString);
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("DatabaseName") ? builder["DatabaseName"].ToString() ?? "N/A" : "N/A";
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Host") && builder["Host"]?.ToString() != null ? ParseHost(builder["Host"].ToString()!) : "N/A";
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Host") && builder["Host"]?.ToString() != null ? ParsePort(builder["Host"].ToString()!) : "N/A";
    }

    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Uid") ? builder["Uid"].ToString() ?? "N/A" : "N/A";
    }

    // Helper method to parse host from Host key
    static string ParseHost(string hostString)
    {
        if (hostString.Contains(":"))
        {
            var parts = hostString.Split(':');
            return parts.Length > 0 ? parts[0] : "N/A";
        }
        return hostString;
    }

    // Helper method to parse port from Host key
    static string ParsePort(string hostString)
    {
        if (hostString.Contains(":"))
        {
            var parts = hostString.Split(':');
            return parts.Length > 1 ? parts[1] : "N/A";
        }
        return "N/A";
    }

    public override Task<CursorBasedResult> GetTables(
        Settings.ConnectionInfoDTO connectonInfo,
        string? query,
        CursorDirection rel,
        string? cursor,
        int limit,
        int offset)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<IDictionary<string, object>>> GetTableFields(
        Settings.ConnectionInfoDTO connectonInfo,
        IDictionary<string, object>? data)
    {
        throw new NotImplementedException();
    }
}
