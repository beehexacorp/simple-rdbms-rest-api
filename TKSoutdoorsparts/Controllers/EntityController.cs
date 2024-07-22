using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DbType = TKSoutdoorsparts.Constants.DbType;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EntityController : ControllerBase
    {
        private readonly IDataHelper _odbcDataHelper;
        private readonly IAppSettings _appSettings;

        public EntityController(IDataHelper odbcDataHelper, IAppSettings appSettings)
        {
            _odbcDataHelper = odbcDataHelper;
            _appSettings = appSettings;
        }
        
        [HttpPost("list")]
        public async Task<IActionResult>  GetData(string query, [FromBody] Dictionary<string, object> @params, DbType dbType)
        {
            var result = await _odbcDataHelper.GetData(query, @params, dbType);
            return Ok(result);
        }
    }
}

// select * from cars order by id limit @limit offset @offset

// select * from cars order by id offset @offset ROWS FETCH NEXT @limit ROWS ONLY