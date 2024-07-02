using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text.Json;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EntityController : ControllerBase
    {
        private readonly IOdbcDataHelper _odbcDataHelper;
        private readonly AppSettings _appSettings;
        private readonly ILogger<EntityController> _logger;

        public EntityController (IOdbcDataHelper odbcDataHelper, AppSettings appSettings, ILogger<EntityController> logger) {
            _odbcDataHelper = odbcDataHelper;
            _appSettings = appSettings;
            _logger = logger;
        }  
        // GET: api/<EntityController>
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string tableName,
            [FromQuery] int? offset,
            [FromQuery] int? limit,
            [FromQuery] string? orderBy)
        {
            _logger.LogInformation($"Get {limit} data from {tableName} started");
            if (offset < 1)
            {
                throw new InvalidDataException("Offset must be > 0");
            }
            var orderByQuery = "";
            if (orderBy != null)
            {
                orderByQuery = $"ORDER BY {orderBy} DESC";
            }

            var query = $"SELECT TOP {limit ?? 10 } START AT {offset ?? 1} * FROM {tableName} {orderByQuery}";
            var dataSet = new DataSet();
            _logger.LogInformation($"Ready to execute query");
            var connectionString = _appSettings.ODBCConnectionString;
            _odbcDataHelper.GetDataSetFromAdapter(dataSet, connectionString, query);
            DataTable dt = dataSet.Tables[0];
            _logger.LogInformation($"Convert data to json");
            var jsonResult = JsonConvert.SerializeObject(dt);
            return Ok(jsonResult);
        }
        // GET: api/<EntityController>
        [HttpGet("listId")]
        public IActionResult GetListId(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromQuery] string idColumn,
            [FromQuery] string tableName,
            [FromQuery] string? orderBy)
        {
            _logger.LogInformation($"Get {limit} {idColumn} from {tableName} started");
            if (offset < 1)
            {
                throw new InvalidDataException("Offset must be > 0");
            }
            var orderByQuery = "";
            if (orderBy != null)
            {
                orderByQuery = $"ORDER BY {orderBy} DESC";
            }
            var query = $"SELECT TOP {limit ?? 10} START AT {offset ?? 1} {idColumn} FROM {tableName} {orderByQuery}";
            var dataSet = new DataSet();
            _logger.LogInformation($"Ready to execute query");
            var connectionString = _appSettings.ODBCConnectionString;
            _odbcDataHelper.GetDataSetFromAdapter(dataSet, connectionString, query);
            DataTable dt = dataSet.Tables[0];
            _logger.LogInformation($"Convert data to json");
            var jsonResult = JsonConvert.SerializeObject(dt);
            return Ok(jsonResult);
        }

        [HttpGet("id")]
        public IActionResult Get(
             [FromQuery] string tableName,
             [FromQuery] string keyName,
             [FromQuery] string keyValue)
        {
            _logger.LogInformation($"Get single data from {tableName} started");
            //var connection = _dbFactory.CreateConnection();
            var query = $"SELECT * FROM {tableName} WHERE {keyName} = '{keyValue}'";
            var dataSet = new DataSet();
            var connectionString = _appSettings.ODBCConnectionString;
            _odbcDataHelper.GetDataSetFromAdapter(dataSet, connectionString, query);
            DataTable dt = dataSet.Tables[0];
            _logger.LogInformation($"Convert data to json");
            var jsonResult = JsonConvert.SerializeObject(dt);
            return Ok(jsonResult);
        }

    }
}
