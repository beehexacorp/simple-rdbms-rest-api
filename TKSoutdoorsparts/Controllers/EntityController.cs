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
        public IActionResult GetData(string query, [FromBody] Dictionary<string, object> @params, DbType dbType)
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();
            var dt = _odbcDataHelper.GetData(query, @params, dbType);
            var result = dt.Result;
            var jsonResult = JsonConvert.SerializeObject(result);
            return Ok(jsonResult);
        }

    }
}

// select * from cars order by id limit @limit offset @offset

// select * from cars order by id offset @offset ROWS FETCH NEXT @limit ROWS ONLY