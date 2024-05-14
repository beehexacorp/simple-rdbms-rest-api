using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Odbc;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EntityController : ControllerBase
    {
        private readonly IDbFactory _dbFactory;
        public EntityController (IDbFactory dbFactory) {
            _dbFactory = dbFactory;
        }  
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<bool> Get(
            [FromQuery] string tableName,
            [FromQuery] string? offset,
            [FromQuery] string? limit)
        {
            var connection = _dbFactory.CreateConnection();
            int qOffset = string.IsNullOrEmpty(offset) ? 0 : int.Parse(offset);
            int qLimit = string.IsNullOrEmpty(limit) ? 0 : int.Parse(limit);
            var query = $"SELECT TOP {qLimit} START AT {qOffset + 1} * FROM {tableName}";
            using OdbcCommand command = new OdbcCommand(query,connection);
            using OdbcDataReader reader = command.ExecuteReader();
            reader.Read();
            //var data = connection.GetSchema("Tables").AsEnumerable().Where(r => r.Field<string>("TABLE_TYPE") = "TABLE");
            DataTable tables = connection.GetSchema(tableName);
            
            return true;
        } 

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
