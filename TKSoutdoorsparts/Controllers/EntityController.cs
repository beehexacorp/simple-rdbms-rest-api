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
public class EntityController : ControllerBase
{
    private readonly IAppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;

    public EntityController(IAppSettings appSettings, IServiceProvider serviceProvider)
    {
        _appSettings = appSettings;
        _serviceProvider = serviceProvider;
    }

    // TODO: /api/sql/update
    // TODO: /api/sql/insert

    // /api/sql/query
    [HttpPost("query")]
    public async Task<IActionResult> GetData(
        [FromBody, Required] EntityRequestMetadata entityRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var dbHelper = _serviceProvider.GetRequiredKeyedService<IDataHelper>(entityRequest.DbType);
        string query = dbHelper.BuildQuery(entityRequest);

        var decodedQuery = HttpUtility.UrlDecode(query);

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

        var result = await dbHelper.GetData(query, entityRequest.@params);
        return Ok(result);
    }
}
