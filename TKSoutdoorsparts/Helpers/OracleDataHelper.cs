using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class OracleDataHelper : BaseDataHelper
{
    public OracleDataHelper() : base() { }

    public override DbType DbType => DbType.ORACLE;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override System.Data.IDbConnection CreateConnection()
    {
        throw new NotImplementedException();
    }
}


