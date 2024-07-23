
using System.Data;
using TKSoutdoorsparts.Factory;
using Dapper;
using DbType = TKSoutdoorsparts.Constants.DbType;

namespace TKSoutdoorsparts.Helpers;
public class PostgresDataHelper : BaseDataHelper
{
    public PostgresDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override DbType DbType => DbType.POSTGRES;

    public override string BuildQuery(string tableName, IEnumerable<string>? fields, IEnumerable<string>? conditions, string? orderBy, Dictionary<string, object>? @params)
    {
        var pgFields = fields != null && fields.Any() ? string.Join(", ", fields) : "*";
        var pgConditions = conditions != null && conditions.Any() ? string.Join(", ", conditions.Select(c => $"{c} = @{c}")) : "";
        pgConditions = !string.IsNullOrWhiteSpace(pgConditions) ? $"where {pgConditions}" : "";
        orderBy = !string.IsNullOrWhiteSpace(orderBy) ? $"order by {orderBy}" : "";
        var query = $@"select {pgFields} 
from {tableName} 
{pgConditions}
{orderBy} 
limit @limit 
offset @offset";
        return query;
    }
}