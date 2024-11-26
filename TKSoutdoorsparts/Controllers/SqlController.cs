using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleRDBMSRestfulAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SqlController(IAppSettings appSettings, IServiceProvider serviceProvider) : ControllerBase
{

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

        var connectonInfo = appSettings.GetConnectionInfo();
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var result = await dbHelper.GetData(decodedQuery, queryRequest.@params);
        return Ok(result);
    }
}
