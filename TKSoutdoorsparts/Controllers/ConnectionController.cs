using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleRDBMSRestfulAPI.Constants;
using SimpleRDBMSRestfulAPI.Helpers;
using SimpleRDBMSRestfulAPI.Models;
using SimpleRDBMSRestfulAPI.Settings;

namespace SimpleRDBMSRestfulAPI.Controllers;

public class ConnectionInfoRequest
{
    public DbType DbType { get; set; } = DbType.MYSQL;
    public string ConnectionString { get; set; } = null!;
}

[Route("api/[controller]")]
[ApiController]
public class ConnectionController(IMapper autoMapper, IAppSettings appSettings, IServiceProvider serviceProvider) : ControllerBase
{

    [HttpGet("db-type")]
    public async Task<IActionResult> GetDbTypes()
    {
        return Ok(await Task.FromResult(Enum.GetValues<DbType>().ToDictionary(v => (int)v, v => v.ToString())));
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var connectionInfos = appSettings.GetConnectionInfos();
        return Ok(await Task.FromResult(connectionInfos.Select(connectionInfo => autoMapper.Map<ConnectionInfoViewModel>(connectionInfo))));
    }
    [HttpGet("{connectionId}")]
    public async Task<IActionResult> GetConnection(Guid connectionId)
    {
        var connectionInfo = appSettings.GetConnectionInfo(connectionId);
        return Ok(await Task.FromResult(autoMapper.Map<ConnectionInfoViewModel>(connectionInfo)));
    }

    [HttpPost("connect")]
    public async Task<IActionResult> TestConnection(
        [FromBody, Required] ConnectionInfoRequest req
    )
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(req.DbType);
        await dbHelper.ConnectAsync(req.ConnectionString);
        return Ok();
    }

    [HttpPost("{connectionId}/connect")]
    public async Task<IActionResult> TestConnectionFromConfigs(Guid connectionId)
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var connectionConfig = appSettings.GetConnectionInfo(connectionId);
        if (connectionConfig == null)
        {
            throw new Exception($"The connection {connectionId} does not exist.");
        }
        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(connectionConfig.DbType);
        await dbHelper.ConnectAsync(connectionConfig.GetConnectionString()!);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SaveConnection(
        [FromBody, Required] ConnectionInfoRequest req
    )
    {
        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var dbHelper = serviceProvider.GetRequiredKeyedService<IDataHelper>(req.DbType);
        await dbHelper.ConnectAsync(req.ConnectionString);

        await appSettings.SaveConnectionAsync(req.DbType, req.ConnectionString);
        return Ok();
    }
}
