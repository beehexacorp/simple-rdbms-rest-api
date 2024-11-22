using System.Data.SqlClient;
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

    public override System.Data.IDbConnection CreateConnection()
    {
        return new SqlConnection(_appSettings.ConnectionString);
    }
}
