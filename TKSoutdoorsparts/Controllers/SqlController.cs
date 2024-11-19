using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TKSoutdoorsparts.Constants;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Models;
using TKSoutdoorsparts.Settings;

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
    public async Task<IActionResult> GetData([FromBody, Required] QueryRequestMetadata queryRequest)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var decodedQuery = HttpUtility.UrlDecode(queryRequest.Query);

        var dangerousCommandsRegex = new Regex(
            RegexConstants.dangerousCommands,
            RegexOptions.IgnoreCase
        );
        if (dangerousCommandsRegex.IsMatch(decodedQuery))
        {
            return BadRequest(new { error = "Query contains forbidden SQL commands." });
        }

        var embeddedStringRegex = new Regex(
            RegexConstants.embeddedString,
            RegexOptions.IgnoreCase
        );
        if (embeddedStringRegex.IsMatch(decodedQuery))
        {
            return BadRequest(new { error = "Query contains forbidden SQL commands." });
        }

        var dbHelper = _serviceProvider.GetRequiredKeyedService<IDataHelper>(queryRequest.DbType);
        var result = await dbHelper.GetData(decodedQuery, queryRequest.@params);
        return Ok(result);
    }
}
