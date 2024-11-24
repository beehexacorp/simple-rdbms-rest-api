using System.Data.SqlClient;
using Dapper;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;

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

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
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
        return new SqlConnection(!string.IsNullOrWhiteSpace(connectionString) ? connectionString : _appSettings.GetConnectionString());
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return builder.InitialCatalog;
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return ParseHost(builder.DataSource);
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
        return ParsePort(builder.DataSource);
    }

    public override string GetUser(byte[] encryptedConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(encryptedConnectionString.DecryptAES());
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

    public override Task<CursorBasedResult> GetTables(string? query, CursorDirection rel, string? cursor, int limit, int offset)
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<IDictionary<string, object>>> GetTableFields(IDictionary<string, object>? data)
    {
        throw new NotImplementedException();
    }
}