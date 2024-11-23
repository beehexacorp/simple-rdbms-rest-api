using System.Data;
using System.Text.Json;
using Dapper;
using SimpleRDBMSRestfulAPI.Models;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public abstract class BaseDataHelper : IDataHelper
{
    public BaseDataHelper() { }

    public abstract DbType DbType { get; }

    public abstract string BuildQuery(EntityRequestMetadata request);
    public abstract Task ConnectAsync(string connectionString);

    public abstract IDbConnection CreateConnection(string? connectionString = null);

    public virtual async Task<IEnumerable<IDictionary<string, object>>> GetData(
        string query,
        Dictionary<string, object>? @params
    )
    {
        var convertedParams = @params?.ToDictionary(
            kvp => kvp.Key,
            kvp =>
                kvp.Value is JsonElement jsonElement ? ConvertJsonElement(jsonElement) : kvp.Value
        );

        using IDbConnection connection = CreateConnection();
        var result = await connection.QueryAsync<dynamic>(query, convertedParams);
        var data = result.Cast<IDictionary<string, object>>().ToList();
        return data;
    }

    public abstract string GetDatabase(byte[] encryptedConnectionString);
    public abstract string GetHost(byte[] encryptedConnectionString);
    public abstract string GetPort(byte[] encryptedConnectionString);
    public abstract string GetUser(byte[] encryptedConnectionString);


    protected virtual object ConvertJsonElement(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.String:
                return jsonElement.GetString() ?? string.Empty;
            case JsonValueKind.Number:
                return jsonElement.GetDouble(); // or GetInt32, GetInt64, etc.
            case JsonValueKind.True:
            case JsonValueKind.False:
                return jsonElement.GetBoolean();
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                return jsonElement.GetRawText(); // Returns the JSON string representation
            default:
                return jsonElement.ToString();
        }
    }
}
