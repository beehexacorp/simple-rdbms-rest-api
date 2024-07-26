using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Models;

namespace TKSoutdoorsparts.Helpers;

public class SqlServerDataHelper(IConnectionFactory connectionFactory) : BaseDataHelper(connectionFactory)
{
    public override DbType DbType => DbType.SQL_SERVER;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

}