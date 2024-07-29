using System.Data.SqlClient;
using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;

namespace TKSoutdoorsparts.Helpers;

public class SqlServerDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public SqlServerDataHelper(IAppSettings appSettings) : base()
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