using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class SQLAnywhereDataHelper : BaseDataHelper
{
    public SQLAnywhereDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory) { }

    public override DbType DbType => DbType.SQLAnywhere;

    public override string BuildQuery(string tableName, IEnumerable<string> fields, IEnumerable<string> conditions, string orderBy, Dictionary<string, object> @params)
    {
        throw new NotImplementedException();
    }
}

