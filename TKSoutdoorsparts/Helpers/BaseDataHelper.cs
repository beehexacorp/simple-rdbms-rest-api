using System.Data;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Models;
using System.Text.Json;

namespace TKSoutdoorsparts.Helpers;

public abstract class BaseDataHelper : IDataHelper
{
    public BaseDataHelper()
    {

    }

    public abstract DbType DbType { get; }

    public abstract string BuildQuery(EntityRequestMetadata request);
    public abstract IDbConnection CreateConnection();


    public virtual async Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object>? @params)
    {
        var convertedParams = @params?.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value is JsonElement jsonElement ? ConvertJsonElement(jsonElement) : kvp.Value
        );

        using IDbConnection connection = CreateConnection();
        var result = await connection.QueryAsync<dynamic>(query, convertedParams);
        var data = result.Cast<IDictionary<string, object>>().ToList();
        return data;
    }

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