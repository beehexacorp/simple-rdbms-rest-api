using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Factory;

namespace TKSoutdoorsparts.Helpers;

public class SqlServerDataHelper(IConnectionFactory connectionFactory) : BaseDataHelper(connectionFactory)
{
    public override DbType DbType => DbType.SQL_SERVER;

    public override string BuildQuery(string tableName, IEnumerable<string>? fields, IEnumerable<string>? conditions, string? orderBy, Dictionary<string, object> @params)
    {
        throw new NotImplementedException();
    }

}