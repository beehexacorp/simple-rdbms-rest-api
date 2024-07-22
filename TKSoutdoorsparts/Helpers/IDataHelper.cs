using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public interface IDataHelper
{
    string BuildQuery(string tableName, IEnumerable<string>? fields, IEnumerable<string>? conditions, string? orderBy, Dictionary<string, object> @params);

    public Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object> @params);
}