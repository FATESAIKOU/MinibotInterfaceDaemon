using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DaemonCore;
using NEXCOMROBOT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                object[] args = null;

                if (command.Length > 2 && command[2] != null && command[2].GetType() == typeof(JArray))
                    args = ConvertArgs((JArray)command[2]);

                DaemonVars.router.DoRoute(target, action, args);
            }
            catch (InvalidCastException e)
            {
                string raw_command = JsonConvert.SerializeObject(command, Formatting.Indented);
                Console.WriteLine($"[HttpInterface]: bad command({raw_command}), error({e.Message})");
            }
            
            return DaemonVars.router.DoRoute("Robot", "GetStatus", null).DoMap();
        }

        private object[] ConvertArgs(JArray raw_args)
        {
            object[] args = new object[raw_args.Count];

            for (int i = 0; i < raw_args.Count; i ++)
            {
                var item = raw_args[i];

                if (item.GetType() == typeof(JArray))
                {
                    double[] pos = new double[6];
                    for (int j = 0; j < 6; j ++)
                        pos[j] = Convert.ToDouble((string) item[j]);
                    
                    args[i] = pos;
                }
                else
                {
                    try
                    {
                        args[i] = Convert.ToInt32((string) item);
                    }
                    catch (FormatException)
                    {
                        args[i] = (string) item;
                    }
                }
            }

            return args;
        }
    }
}
