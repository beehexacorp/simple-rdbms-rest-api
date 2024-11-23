using System.Data.SqlClient;
using Dapper;
using SimpleRDBMSRestfulAPI.Core;
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


    public override System.Data.IDbConnection CreateConnection(string? connectionString = null)
    {
        return new SqlConnection(!string.IsNullOrWhiteSpace(connectionString) ? connectionString : _appSettings.GetConnectionString());
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

    public override Task<CursorBasedResult> GetTables(string? query, CursorDirection rel, string? cursor, int limit, int offset)
    {
        throw new NotImplementedException();
    }


    public override string GetUser(byte[] encryptedConnectionString)
    {
        return new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(encryptedConnectionString.DecryptAES()).UserID;
    }
}
