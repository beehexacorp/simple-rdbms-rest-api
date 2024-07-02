using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using TKSoutdoorsparts.Helpers;
using TKSoutdoorsparts.Settings;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchemaController : ControllerBase
    {
        private readonly IOdbcDataHelper _odbcDataHelper;
        private readonly AppSettings _appSettings;
        public SchemaController(IOdbcDataHelper odbcDataHelper, AppSettings appSettings)
        {
            _odbcDataHelper = odbcDataHelper;
            _appSettings = appSettings;
        }
        // GET: api/<SchemaController>
        [HttpGet]
        public IActionResult GetSchema()
        {
            try
            {
                var dataSet = new DataSet();
                var connectionString = _appSettings.ODBCConnectionString;
                _odbcDataHelper.GetSchema(dataSet, connectionString);
                DataTable dt = dataSet.Tables[0];
                var jsonResult = JsonConvert.SerializeObject(dt);
                return Ok(jsonResult);
            } catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }


        }
    }
}
