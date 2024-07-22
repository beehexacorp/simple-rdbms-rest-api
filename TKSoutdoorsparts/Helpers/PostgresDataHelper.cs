
using System.Data;
using TKSoutdoorsparts.Factory;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;
public class PostgresDataHelper : BaseDataHelper
{
    public PostgresDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override DbType DbType => DbType.POSTGRES;
}