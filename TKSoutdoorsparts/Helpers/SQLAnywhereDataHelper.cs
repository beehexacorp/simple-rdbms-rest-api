﻿using TKSoutdoorsparts.Factory;
using TKSoutdoorsparts.Models;
using DbType = TKSoutdoorsparts.Constants.DbType;
namespace TKSoutdoorsparts.Helpers;

public class SqlAnywhereDataHelper : BaseDataHelper
{
    public SqlAnywhereDataHelper(IConnectionFactory connectionFactory) : base(connectionFactory) { }

    public override DbType DbType => DbType.SQLAnywhere;
    public override string BuildQuery(EntityRequestMetadata request)
    {
        Dictionary<string, object> @params = request.@params;
        @params = @params ?? new Dictionary<string, object>();
        if (!@params.ContainsKey("top"))
        {
            throw new ArgumentNullException("The @top param is required");
        }

        if (!@params.ContainsKey("startAt"))
        {
            throw new ArgumentNullException("The @startAt param is required");
        }
        var topValue = request.@params["top"];
        var startAtValue = request.@params["startAt"];
        var pgFields = request.Fields != null && request.Fields.Any() ? string.Join(", ", request.Fields) : "*";
        var pgConditions = request.Conditions != null && request.Conditions.Any() ? string.Join(", ", request.Conditions.Select(c => $"{c} = @{c}")) : "";
        pgConditions = !string.IsNullOrWhiteSpace(pgConditions) ? $"WHERE {pgConditions}" : "";
        request.OrderBy = !string.IsNullOrWhiteSpace(request.OrderBy) ? $"ORDER BY {request.OrderBy}" : "";
        var query = $@"SELECT TOP {topValue} START AT {startAtValue} {pgFields} FROM {request.TableName} {pgConditions} {request.OrderBy}";
        return query;
    }
}