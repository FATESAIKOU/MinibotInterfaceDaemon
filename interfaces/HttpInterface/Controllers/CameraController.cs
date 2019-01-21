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
    public class CameraController : Controller
    {
        // GET api/values
        [HttpGet("{id}")]
        public CameraStatusReadable Get(int id)
        {
            object[] args = new object[1];

            args[0] = id;
            return ((CameraStatus)DaemonVars.router.DoRoute("Camera", "Capture", args)).DoMap();    
        }
    }
}
