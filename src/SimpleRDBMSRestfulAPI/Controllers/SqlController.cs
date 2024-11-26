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
public class SqlController(IAppSettings appSettings, IServiceProvider serviceProvider, ISqlInjectionHelper sqlInjectionHelper) : ControllerBase
{

    // TODO: /api/sql/update
    // TODO: /api/sql/insert

    // /api/sql/query
    [HttpPost("{connectionId}/query")]
    public async Task<IActionResult> GetData(
        [FromRoute] Guid connectionId,
        [FromBody, Required] QueryRequestMetadata queryRequest)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var decodedQuery = HttpUtility.UrlDecode(queryRequest.Query);
        await sqlInjectionHelper.EnsureValid(decodedQuery);

        var connectonInfo = appSettings.GetConnectionInfo(connectionId);
        if (connectonInfo?.ConnectionString == null)
        {
            throw new Exception("Please ask the API Owner to configure the database connection.");
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectonInfo.DbType);
        var result = await dbHelper.GetData(connectonInfo, decodedQuery, queryRequest.@params);
        return Ok(result);
    }
}
