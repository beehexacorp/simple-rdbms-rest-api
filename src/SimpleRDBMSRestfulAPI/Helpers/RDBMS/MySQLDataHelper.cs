using System.Data.SqlClient;
using Dapper;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public class MySqlDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public MySqlDataHelper(IAppSettings appSettings)
        : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.MYSQL;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override async Task ConnectAsync(string connectionString)
    {
        using var conn = CreateConnection(connectionString);
        conn.Open();
        var _ = await conn.QueryFirstOrDefaultAsync<bool?>("SELECT 1 FROM information_schema.tables LIMIT 1;");
    }


    public override System.Data.IDbConnection CreateConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        return new SqlConnection(connectionString);
    }

    public override string GetDatabase(byte[] encryptedConnectionString)
    {
        return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(encryptedConnectionString.DecryptAES()).Database;
    }

    public override string GetHost(byte[] encryptedConnectionString)
    {

        return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(encryptedConnectionString.DecryptAES()).Server;
    }

    public override string GetPort(byte[] encryptedConnectionString)
    {
        return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(encryptedConnectionString.DecryptAES()).Port.ToString();
    }

    public override string GetUser(byte[] encryptedConnectionString)
    {
        return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(encryptedConnectionString.DecryptAES()).UserID;
    }

    public override Task<IEnumerable<IDictionary<string, object>>> GetTableFields(
        Settings.ConnectionInfoDTO connectonInfo,
        IDictionary<string, object>? data)
    {
        throw new NotImplementedException();
    }

    public override Task<CursorBasedResult> GetTables(
        Settings.ConnectionInfoDTO connectonInfo,
        string? query, CursorDirection rel, string? cursor, int limit, int offset)
    {
        throw new NotImplementedException();
    }
}
