using System.Data.Odbc;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;
using DbType = TKSoutdoorsparts.Constants.DbType;
namespace TKSoutdoorsparts.Helpers;

public class SqlAnywhereDataHelper : BaseDataHelper
{
    private readonly IAppSettings _appSettings;


    public SqlAnywhereDataHelper(IAppSettings appSettings) : base()
    {
        _appSettings = appSettings;
    }

    public override DbType DbType => DbType.SQL_ANYWHERE;
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

    public override System.Data.IDbConnection CreateConnection()
    {
        return new OdbcConnection(_appSettings.ConnectionString);
    }

}