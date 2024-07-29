using System.Data.Odbc;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class OracleDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;

    public OracleDataHelper(IAppSettings appSettings) : base() {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.ORACLE;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override System.Data.IDbConnection CreateConnection()
    {
        return new OdbcConnection(_appSettings.ConnectionString);
    }
}


