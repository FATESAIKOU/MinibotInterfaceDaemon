using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaemonCore;
using NEXCOMROBOT;
using Newtonsoft.Json;

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
        public RobotStatusReadable Put(int id, [FromBody]object[] command)
        {
            try
            {
                string target = (string) command[0];
                string action = (string) command[1];
                object[] args = (object[]) command[2];

                DaemonVars.router.DoRoute(target, action, args);
            }
            catch (InvalidCastException e)
            {
                string raw_command = JsonConvert.SerializeObject(command, Formatting.Indented);
                Console.WriteLine($"[HttpInterface]: bad command({raw_command}), error({e.Message})");
            }
            
            return DaemonVars.router.DoRoute("Robot", "GetStatus", null).DoMap();
        }
    }
}
