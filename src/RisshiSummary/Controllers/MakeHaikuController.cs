using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using RisshiSummary.Action;

namespace RisshiSummary.Controllers
{
    [Route("RisshiSummary/[controller]")]
    public class MakeHaikuController : Controller
    {
        // GET
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var u=Request;
            return new string[] { "value1", "value2" };
        }

        // POST api/values
        [HttpPost()]
        public HaikuResult Post([FromBody]PostedData inpu)
        {
            try
            {
                KuMaker ku = new KuMaker();
                return ku.CreateKu(inpu.inputedTxt, inpu.mode);
            }
            catch (Exception e)
            {
                Log.OutputExceptionLog(e);
                throw e;
            }
        }

        public class PostedData
        {
            public string inputedTxt { get; set; }
            public string mode { get; set; }
        }

    }
}
