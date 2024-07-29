using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class OracleDataHelper : BaseDataHelper
{
    public OracleDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory) { }

    public override DbType DbType => DbType.ORACLE;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

}


