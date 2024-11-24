using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Models;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public interface IDataHelper
{
    string BuildQuery(EntityRequestMetadata request);
    Task ConnectAsync(string connectionString);
    public Task<IEnumerable<IDictionary<string, object>>> GetData(
        string query,
        Dictionary<string, object>? @params
    );
    string GetDatabase(byte[] encryptedConnectionString);
    string GetHost(byte[] encryptedConnectionString);
    string GetPort(byte[] encryptedConnectionString);
    string GetUser(byte[] encryptedConnectionString);
    Task<CursorBasedResult> GetTables(string? query, CursorDirection rel, string? cursor, int limit, int offset);
    Task<IEnumerable<IDictionary<string, object>>> GetTableFields(IDictionary<string, object>? data);

}
