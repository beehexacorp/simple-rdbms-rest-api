﻿using Microsoft.AspNetCore.Mvc;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;
using System.ComponentModel.DataAnnotations;
using TKSoutdoorsparts.Models;

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
        // TODO: use regular expression to throw error for UPDATE, INSERT, DELETE, DROP commands
        // TODO: use regular expression to throw error for queries that combine string inside, e.g.: select * from x where a = '1'
        
        var dbHelper = _serviceProvider.GetRequiredKeyedService<IDataHelper>(entityRequest.DbType);
        string query = dbHelper.BuildQuery(entityRequest);
        var result = await dbHelper.GetData(query, entityRequest.@params);
        return Ok(result);
    }
}