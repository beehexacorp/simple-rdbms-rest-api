using System.Data.SqlClient;
using SimpleRDBMSRestfulAPI.Constants;
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

    public override System.Data.IDbConnection CreateConnection()
    {
        return new SqlConnection(_appSettings.ConnectionString);
    }
}
