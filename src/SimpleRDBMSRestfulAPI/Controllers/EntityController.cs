using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Libs;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SimpleRDBMSRestfulAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntityController(IAppSettings appSettings, IServiceProvider serviceProvider, ISqlInjectionHelper sqlInjectionHelper) : ControllerBase
{
    [HttpGet("{connectionId}/tables")]
    public async Task<IActionResult> GetTables(
        [FromRoute] Guid connectionId,
        [FromQuery] CursorDirection rel = CursorDirection.Next,
        [FromQuery(Name = "q")] string? query = null,
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0)
    {
        var connectonInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var results = await dbHelper.GetTables(connectonInfo, query, rel, cursor, limit, offset);
        return Ok(results);
    }

    [HttpGet("{connectionId}/tables/detail")]
    public async Task<IActionResult> GetDetail(
        [FromRoute] Guid connectionId,
        [FromQuery] string detailEncoded)
    {
        if (string.IsNullOrWhiteSpace(detailEncoded))
        {
            return BadRequest("Detail parameter is required.");
        }

        // Decode the Base64 string
        var jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(detailEncoded));

        // Deserialize into a dictionary
        var data = JsonSerializer.Deserialize<IDictionary<string, object>>(jsonString);

        var connectonInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var results = await dbHelper.GetTableFields(connectonInfo, data);
        // Return the parsed object
        return Ok(results);
    }

    // /api/sql/query
    [HttpPost("{connectionId}/query")]
    public async Task<IActionResult> GetData(
        [FromRoute] Guid connectionId,
        [FromBody, Required] EntityRequestMetadata entityRequest
    )
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var connectonInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        string query = dbHelper.BuildQuery(entityRequest);

        var decodedQuery = HttpUtility.UrlDecode(query);
        await sqlInjectionHelper.EnsureValid(decodedQuery);

        var result = await dbHelper.GetData(connectonInfo, query, entityRequest.@params);
        return Ok(result);
    }
}
