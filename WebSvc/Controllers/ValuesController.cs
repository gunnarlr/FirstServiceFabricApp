using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FirstStatefulService.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace WebSvc.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            ICounter counter =
                    ServiceProxy.Create<ICounter>(0, new Uri("fabric:/FirstServiceFabricApp/FirstStatefulService"));
            long count = await counter.GetCountAsync();

            return new string[] { count.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
