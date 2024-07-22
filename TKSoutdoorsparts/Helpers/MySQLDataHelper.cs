using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class MySQLDataHelper : BaseDataHelper
{
    public MySQLDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory) { }

    public override DbType DbType => DbType.MYSQL;

    public override string BuildQuery(string tableName, IEnumerable<string> fields, IEnumerable<string> conditions, string orderBy, Dictionary<string, object> @params)
    {
        throw new NotImplementedException();
    }

}


