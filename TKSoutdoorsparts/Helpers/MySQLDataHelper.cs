using System.Data.SqlClient;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class MySqlDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;
    
    public MySqlDataHelper(IAppSettings appSettings) : base() {
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