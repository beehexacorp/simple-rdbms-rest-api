using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers;

[Route("api/[controller]")]
[ApiController]

public class SqlController : ControllerBase
{
    private readonly IAppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;

    public SqlController(IAppSettings appSettings, IServiceProvider serviceProvider)
    {
        _appSettings = appSettings;
        _serviceProvider = serviceProvider;
    }
    // TODO: /api/sql/update
    // TODO: /api/sql/insert

    // /api/sql/query
    [HttpPost("query")]
    public async Task<IActionResult> GetData(
        [FromBody, Required] string query,
        [FromBody, Required] Dictionary<string, object> @params,
        [FromBody, Required] DbType dbType)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        // TODO: use regular expression to throw error for UPDATE, INSERT, DELETE, DROP commands
        // TODO: use regular expression to throw error for queries that combine string inside, e.g.: select * from x where a = '1'
        IDataHelper dbHelper;
        switch (dbType)
        {
            case DbType.SQLAnywhere:
                dbHelper = _serviceProvider.GetRequiredService<SQLAnywhereDataHelper>();
                break;
            case DbType.POSTGRES:
                dbHelper = _serviceProvider.GetRequiredService<PostgresDataHelper>();
                break;
            case DbType.MYSQL:
                dbHelper = _serviceProvider.GetRequiredService<MySQLDataHelper>();
                break;
            default:
                throw new NotImplementedException($"The dbType {dbType} is not supported yet.");
        }
        var result = await dbHelper.GetData(query, @params);
        return Ok(result);
    }
}

// select * from cars order by id limit @limit offset @offset

// select * from cars order by id offset @offset ROWS FETCH NEXT @limit ROWS ONLY