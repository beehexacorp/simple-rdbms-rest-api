using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Dapper;
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
        var connectionInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectionInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectionInfo.DbType);
        var results = await dbHelper.GetTables(connectionInfo, query, rel, cursor, limit, offset);
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

        var connectionInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectionInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectionInfo.DbType);
        var results = await dbHelper.GetTableFields(connectionInfo, data);
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

        var connectionInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectionInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectionInfo.DbType);
        var queryData = dbHelper.BuildQuery(entityRequest);

        queryData.query = HttpUtility.UrlDecode(queryData.query);
        await sqlInjectionHelper.EnsureValid(queryData.query);

        var result = await dbHelper.GetData(connectionInfo, queryData);
        return Ok(result);
    }

    // -----------------------------
    // CRUD: READ
    // -----------------------------
    [HttpGet("{connectionId}/tables/{tableName}")]
    public async Task<IActionResult> Read(
    [FromRoute] Guid connectionId,
    [FromRoute] string tableName)
    {
        var connInfo = appSettings.GetConnectionInfo(connectionId);
        if (connInfo?.ConnectionString == null)
            throw new Exception("Database not configured.");

        // Parse query string to Dictionary<string, object>
        var filters = HttpContext.Request.Query
            .ToDictionary(kv => kv.Key, kv => (object)kv.Value.ToString());

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connInfo.DbType);
        var rows = await dbHelper.ReadRecord(connInfo.ConnectionString, tableName, filters);

        return Ok(rows);
    }


    // -----------------------------
    // CRUD: CREATE (Insert)
    // -----------------------------
    [HttpPost("{connectionId}/tables/{tableName}")]
    public async Task<IActionResult> Create(
        [FromRoute] Guid connectionId,
        [FromRoute] string tableName,
        [FromBody] Dictionary<string, object> body)
    {
        if (body == null || body.Count == 0)
            return BadRequest("Request body cannot be empty.");

        var connInfo = appSettings.GetConnectionInfo(connectionId);
        if (connInfo?.ConnectionString == null)
            throw new Exception("Please ask the API Owner to configure the database connection.");

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connInfo.DbType);

        var result = await dbHelper.CreateRecord(connInfo.ConnectionString, tableName, body);
        return Ok((IDictionary<string, object>)result);
    }

    // -----------------------------
    // CRUD: UPDATE 
    // -----------------------------
    [HttpPut("{connectionId}/tables/{tableName}")]
    public async Task<IActionResult> Update(
    [FromRoute] Guid connectionId,
    [FromRoute] string tableName,
    [FromBody] Dictionary<string, object> body)
    {
        if (body == null || body.Count == 0)
            return BadRequest("Request body cannot be empty.");

        var connInfo = appSettings.GetConnectionInfo(connectionId);
        if (connInfo?.ConnectionString == null)
            throw new Exception("Please ask the API Owner to configure the database connection.");

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connInfo.DbType);

        var filters = HttpContext.Request.Query
            .ToDictionary(kv => kv.Key, kv => (object)kv.Value.ToString());

        // Call the SQLServerDataHelper's Update method
        var updatedRecord = await dbHelper.UpdateRecord(
            connInfo.ConnectionString,
            tableName,
            body,
            filters
        );

        return Ok((IDictionary<string, object>)updatedRecord);
    }

    // -----------------------------
    // CRUD: DELETE 
    // -----------------------------
    [HttpDelete("{connectionId}/tables/{tableName}")]
    public async Task<IActionResult> Delete(
    [FromRoute] Guid connectionId,
    [FromRoute] string tableName)
    {
        var connInfo = appSettings.GetConnectionInfo(connectionId);
        if (connInfo?.ConnectionString == null)
            throw new Exception("Please ask the API Owner to configure the database connection.");

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connInfo.DbType);
        var filters = HttpContext.Request.Query
            .ToDictionary(kv => kv.Key, kv => (object)kv.Value.ToString());

        // Call the SQLServerDataHelper's Delete method
        var deletedRecord = await dbHelper.DeleteRecord(
        connInfo.ConnectionString,
        tableName,
        filters
    );

        return Ok((IDictionary<string, object>)deletedRecord);
    }
}
