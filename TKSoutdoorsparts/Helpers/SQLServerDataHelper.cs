using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Models;

namespace TKSoutdoorsparts.Helpers;

public class SqlServerDataHelper() : BaseDataHelper()
{
    public override DbType DbType => DbType.SQL_SERVER;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override System.Data.IDbConnection CreateConnection()
    {
        throw new NotImplementedException();
    }
}