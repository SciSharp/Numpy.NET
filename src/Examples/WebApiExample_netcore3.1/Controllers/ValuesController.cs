using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Numpy;
using Python.Runtime;

namespace WebApiExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            using (Py.GIL()) {
                var array = new float[2, 2] {{DateTime.Now.Minute, DateTime.Now.Second}, {DateTime.Now.Millisecond, (float)Math.PI}};
                var ndArray = new NDarray(array);
                //return ndArray.ToString();
                return ndArray.repr;
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            using (Py.GIL())
            {
                var array = new float[2, 2] { { DateTime.Now.Minute, DateTime.Now.Second }, { DateTime.Now.Millisecond, (float)Math.PI } };
                var ndArray = new NDarray(array);
                //return ndArray.ToString();
                return ndArray.repr;
            }
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
