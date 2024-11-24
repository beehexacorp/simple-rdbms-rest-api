using System.Data.Odbc;
using Dapper;
using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public class OracleDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public OracleDataHelper(IAppSettings appSettings)
        : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.ORACLE;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override async Task ConnectAsync(string connectionString)
    {
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var _ = await conn.QueryFirstOrDefaultAsync<bool?>("SELECT 1 FROM all_tables;");
    }


    public override System.Data.IDbConnection CreateConnection(string? connectionString = null)
    {
        return new OdbcConnection(!string.IsNullOrWhiteSpace(connectionString) ? connectionString : _appSettings.GetConnectionString());
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Dbq") && builder["Dbq"]?.ToString() != null ? ParseDatabaseName(builder["Dbq"].ToString()!) : "N/A";
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Dbq") && builder["Dbq"]?.ToString() != null ? ParseHost(builder["Dbq"].ToString()!) : "N/A";
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Dbq") && builder["Dbq"]?.ToString() != null ? ParsePort(builder["Dbq"].ToString()!) : "N/A";
    }

    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new OdbcConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.ContainsKey("Uid") ? builder["Uid"].ToString() ?? "N/A" : "N/A";
    }

    // Helper method to parse database name
    static string ParseDatabaseName(string dbq)
    {
        if (dbq.Contains("/"))
        {
            var parts = dbq.Split('/');
            return parts.Length > 1 ? parts[1] : "N/A";
        }
        return "N/A";
    }

    // Helper method to parse host
    static string ParseHost(string dbq)
    {
        if (dbq.Contains(":"))
        {
            var parts = dbq.Split(':');
            return parts.Length > 0 ? parts[0] : "N/A";
        }
        return "N/A";
    }

    // Helper method to parse port
    static string ParsePort(string dbq)
    {
        if (dbq.Contains(":"))
        {
            var parts = dbq.Split(':');
            if (parts.Length > 1 && parts[1].Contains("/"))
            {
                return parts[1].Split('/')[0];
            }
        }
        return "N/A";
    }

    public override Task<CursorBasedResult> GetTables(string? query, CursorDirection rel, string? cursor, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<IDictionary<string, object>>> GetTableFields(IDictionary<string, object>? data)
    {
        throw new NotImplementedException();
    }
}
