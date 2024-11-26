using System.Data;
using System.Text.Json;
using Dapper;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Models;
using DbType = SimpleRDBMSRestfulAPI.Constants.DbType;

namespace SimpleRDBMSRestfulAPI.Helpers;

public abstract class BaseDataHelper : IDataHelper
{
    public BaseDataHelper() { }

    public abstract DbType DbType { get; }

    public abstract string BuildQuery(EntityRequestMetadata request);
    public abstract Task ConnectAsync(string connectionString);

    public abstract IDbConnection CreateConnection(string connectionString);

    public virtual async Task<IEnumerable<IDictionary<string, object>>> GetData(
        Settings.ConnectionInfoDTO connectonInfo,
        string query,
        Dictionary<string, object>? @params
    )
    {
        var convertedParams = @params?.ToDictionary(
            kvp => kvp.Key,
            kvp =>
                kvp.Value is JsonElement jsonElement ? ConvertJsonElement(jsonElement) : kvp.Value
        );

        var connectionString = Convert.FromBase64String(connectonInfo.ConnectionString).DecryptAES();
        using IDbConnection connection = CreateConnection(connectionString);
        var result = await connection.QueryAsync<dynamic>(query, convertedParams);
        var data = result.Cast<IDictionary<string, object>>().ToList();
        return data;
    }

    public abstract string GetDatabase(byte[] encryptedConnectionString);
    public abstract string GetHost(byte[] encryptedConnectionString);
    public abstract string GetPort(byte[] encryptedConnectionString);
    public abstract string GetUser(byte[] encryptedConnectionString);

    public abstract Task<IEnumerable<IDictionary<string, object>>> GetTableFields(
        Settings.ConnectionInfoDTO connectonInfo,
        IDictionary<string, object>? data);

    public abstract Task<CursorBasedResult> GetTables(
        Settings.ConnectionInfoDTO connectonInfo,
        string? query,
        CursorDirection rel,
        string? cursor,
        int limit,
        int offset);



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
