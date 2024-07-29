using System.Data;
using TKSoutdoorsparts.Factory;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Models;
using System.Text.Json;

namespace TKSoutdoorsparts.Helpers;

public abstract class BaseDataHelper : IDataHelper
{
    protected readonly IConnectionFactory ConnectionFactory;
    public BaseDataHelper(IConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public abstract DbType DbType { get; }

    public abstract string BuildQuery(EntityRequestMetadata request);
    
    public virtual async Task<IEnumerable<IDictionary<string, object>>> GetData(string query, Dictionary<string, object>? @params)
    {
        var convertedParams = @params?.ToDictionary(
        kvp => kvp.Key, 
        kvp => kvp.Value is JsonElement jsonElement ? ConvertJsonElement(jsonElement) : kvp.Value
    );
        
        using IDbConnection connection = ConnectionFactory.CreateConnection(DbType);
        var result = await connection.QueryAsync<dynamic>(query, convertedParams);
        var data = result.Cast<IDictionary<string, object>>().ToList();
        return data;
    }

    private object ConvertJsonElement(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.String:
                return jsonElement.GetString();
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