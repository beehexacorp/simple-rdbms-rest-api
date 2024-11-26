using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Core;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleRDBMSRestfulAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntityController(IAppSettings appSettings, IServiceProvider serviceProvider) : ControllerBase
{
    [HttpGet()]
    public async Task<IActionResult> GetTables(
        [FromQuery] CursorDirection rel = CursorDirection.Next,
        [FromQuery(Name = "q")] string? query = null,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0)
    {
        var connectonInfo = appSettings.GetConnectionInfo();
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var results = await dbHelper.GetTables(query, rel, cursor, limit, offset);
        return Ok(results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail([FromQuery] string detailEncoded)
    {
        if (string.IsNullOrWhiteSpace(detailEncoded))
        {
            return BadRequest("Detail parameter is required.");
        }

        // Decode the Base64 string
        var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(detailEncoded));

        // Deserialize into a dictionary
        var data = JsonSerializer.Deserialize<IDictionary<string, object>>(jsonString);

        var connectonInfo = appSettings.GetConnectionInfo();
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var results = await dbHelper.GetTableFields(data);
        // Return the parsed object
        return Ok(results);
    }

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

        var connectonInfo = appSettings.GetConnectionInfo();
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
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
