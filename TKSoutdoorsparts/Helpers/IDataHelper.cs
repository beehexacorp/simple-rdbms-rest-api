using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public interface IDataHelper
{
    string BuildQuery(EntityRequestMetadata request);

    public Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object>? @params);
}