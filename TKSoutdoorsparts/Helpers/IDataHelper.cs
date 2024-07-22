using TKSoutdoorsparts.Factory;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public interface IDataHelper
{
    public Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object> @params);
}