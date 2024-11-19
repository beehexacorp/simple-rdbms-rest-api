using Npgsql;
using System.Data;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;

public class PostgresDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;


    public PostgresDataHelper(IAppSettings appSettings) : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.POSTGRES;

    public override string BuildQuery(EntityRequestMetadata request)
    {
        Dictionary<string, object> @params = request.@params;
        @params = @params ?? new Dictionary<string, object>();
        if (!@params.ContainsKey("limit"))
        {
            throw new ArgumentNullException("The @limit param is required");
        }
        if (!@params.ContainsKey("offset"))
        {
            throw new ArgumentNullException("The @offset param is required");
        }
        var pgFields = request.Fields != null && request.Fields.Any() ? string.Join(", ", request.Fields) : "*";
        var pgConditions = request.Conditions != null && request.Conditions.Any() ? string.Join("AND ", request.Conditions.Select(c => $"{c} = @{c}")) : "";
        pgConditions = !string.IsNullOrWhiteSpace(pgConditions) ? $"where {pgConditions}" : "";
        request.OrderBy = !string.IsNullOrWhiteSpace(request.OrderBy) ? $"order by {request.OrderBy}" : "";
        var query = $@"SELECT {pgFields} 
FROM {request.TableName} 
{pgConditions} 
{request.OrderBy} 
LIMIT @limit 
OFFSET @offset";
        return query;
    }

    public override IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_appSettings.ConnectionString);
    }

}