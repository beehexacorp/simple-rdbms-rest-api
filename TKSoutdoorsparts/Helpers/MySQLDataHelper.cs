using System.Runtime.CompilerServices;
using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class MySqlDataHelper : BaseDataHelper
{
    public MySqlDataHelper() : base() { }

    public override DbType DbType => DbType.MYSQL;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        throw new NotImplementedException();
    }

    public override System.Data.IDbConnection CreateConnection()
    {
        throw new NotImplementedException();
    }
}