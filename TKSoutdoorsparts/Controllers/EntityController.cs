﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Text.Json;
using TKSoutdoorsparts.Adapter;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TKSoutdoorsparts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EntityController : ControllerBase
    {
        private readonly IOdbcDataHelper _odbcDataHelper;
        public EntityController (IOdbcDataHelper odbcDataHelper) {
            _odbcDataHelper = odbcDataHelper;
        }  
        // GET: api/<ValuesController>
        [HttpGet]
        public IActionResult Get(
            [FromQuery] string tableName = "INV",
            [FromQuery] int? offset = 1,
            [FromQuery] int? limit = 100)
        {
            //var connection = _dbFactory.CreateConnection();
            if (offset < 1)
            {
                throw new InvalidDataException("Offset must be > 0");
            }
            var query = $"SELECT TOP {limit} START AT {offset} * FROM {tableName}";
            var dataSet = new DataSet();
            var connectionString = "DRIVER=SQL Anywhere 16;HOST=127.0.0.1:2638;DATABASE=enterprise;Trusted_Connection=Yes;Uid=EXT;Pwd=EXT";
            _odbcDataHelper.GetDataSetFromAdapter(dataSet, connectionString, query);
            DataTable dt = dataSet.Tables[0];
            var jsonResult = JsonConvert.SerializeObject(dt);

            return Ok(jsonResult);
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
