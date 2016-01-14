using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FirstStatefulService.Interface;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Actors;
using VerySimpleActor.Interfaces;

namespace WebSvc.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private const string theApplicationUri = "fabric:/FirstServiceFabricApp";
        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            // Talk to a stateless SERVICE to get the counter
            ICounter counter =
                    ServiceProxy.Create<ICounter>(0, new Uri(theApplicationUri+"/FirstStatefulService"));
            long count = await counter.GetCountAsync();

            // Talk to a stateless ACTOR to get a random number
            string theRandomNum = "42.0";
            var simpleActor = ActorProxy.Create<IVerySimpleActor>(ActorId.NewId(), theApplicationUri);
            theRandomNum = await simpleActor.GetRandomValue();

            return new string[] { String.Format("Hello world! Here's the count : {0} and here's a random number {1:0.00}", count.ToString(), theRandomNum) };
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
