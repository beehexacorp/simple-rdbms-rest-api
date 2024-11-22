using SimpleRDBMSRestfulAPI.Models;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public interface IDataHelper
{
    string BuildQuery(EntityRequestMetadata request);

    public Task<IEnumerable<IDictionary<string, object>>> GetData(
        string query,
        Dictionary<string, object>? @params
    );
}
