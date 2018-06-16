using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaemonCore;
using NEXCOMROBOT;

namespace HttpInterface.Controllers
{
    [Route("api/[controller]")]
    public class RobotController : Controller
    {
        // GET api/values
        [HttpGet("{id}")]
        public RobotStatusReadable Get(int id)
        {
            if (id == 0)
            {
                return DaemonVars.router.DoRoute("Robot", "GetStatus", null).DoMap();    
            }
            else
            {
                return null;
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
    }
}
