using Microsoft.AspNetCore.Mvc;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;
using System.ComponentModel.DataAnnotations;
using TKSoutdoorsparts.Models;

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
        [FromBody, Required] QueryRequestMetadata queryRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }
        
        // TODO: use regular expression to throw error for UPDATE, INSERT, DELETE, DROP commands
        // TODO: use regular expression to throw error for queries that combine string inside, e.g.: select * from x where a = '1'
        DbType dbType = queryRequest.DbType;
        IDataHelper? dbHelper = dbType switch
        {
            DbType.SQLAnywhere => _serviceProvider.GetRequiredService<SqlAnywhereDataHelper>(),
            DbType.POSTGRES => _serviceProvider.GetRequiredService<PostgresDataHelper>(),
            DbType.MYSQL => _serviceProvider.GetRequiredService<MySqlDataHelper>(),
            DbType.SQL_SERVER => _serviceProvider.GetRequiredService<SqlServerDataHelper>(),
            DbType.ORACLE => _serviceProvider.GetRequiredService<OracleDataHelper>(),
            _ => throw new NotImplementedException($"The dbType {dbType} is not supported yet.")
        };
        var result = await dbHelper.GetData(queryRequest.Query, queryRequest.@params);
        return Ok(result);
    }
}