using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using TKSoutdoorsparts.Adapter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchemaController : ControllerBase
    {
        private readonly IOdbcDataHelper _odbcDataHelper;
        public SchemaController(IOdbcDataHelper odbcDataHelper)
        {
            _odbcDataHelper = odbcDataHelper;
        }
        // GET: api/<SchemaController>
        [HttpGet]
        public IActionResult GetSchema()
        {
            var dataSet = new DataSet();
            var connectionString = "DRIVER=SQL Anywhere 16;HOST=127.0.0.1:2638;DATABASE=enterprise;Trusted_Connection=Yes;Uid=EXT;Pwd=EXT";
            _odbcDataHelper.GetSchema(dataSet, connectionString);
            DataTable dt = dataSet.Tables[0];
            var jsonResult = JsonConvert.SerializeObject(dt);
            return Ok(jsonResult);

        }

        // GET api/<SchemaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SchemaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<SchemaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SchemaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
