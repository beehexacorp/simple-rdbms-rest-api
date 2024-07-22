using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class OracleDataHelper : BaseDataHelper
{
    public OracleDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory) { }

    public override DbType DbType => DbType.ORACLE;

    public override string BuildQuery(string tableName, IEnumerable<string> fields, IEnumerable<string> conditions, string orderBy, Dictionary<string, object> @params)
    {
        throw new NotImplementedException();
    }

}


