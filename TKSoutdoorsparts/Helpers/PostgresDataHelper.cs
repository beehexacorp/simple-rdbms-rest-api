using System.Data;
using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;
public class PostgresDataHelper : BaseDataHelper
{
    public PostgresDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override DbType DbType => DbType.POSTGRES;

    public override string BuildQuery(EntityRequestMetadata request)
    {

        var pgFields = request.Fields != null && request.Fields.Any() ? string.Join(", ", request.Fields) : "*";
        var pgConditions = request.Conditions != null && request.Conditions.Any() ? string.Join(", ", request.Conditions.Select(c => $"{c} = @{c}")) : "";
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
}