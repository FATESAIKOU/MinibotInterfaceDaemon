using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DaemonCore;
using DaemonInterface;

namespace HttpInterface
{
    public static class DaemonVars
    {
        public static Route router = new Route();
        public static StreamReader stdin_stream = new System.IO.StreamReader(Console.OpenStandardInput());
        public static StreamWriter stdout_stream = new System.IO.StreamWriter(Console.OpenStandardOutput());

    }


    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start Daemon");

            // Init daemon var
            DaemonVars.router.Start();
            //DaemonVars.router.Start(NEXCOMROBOT.MCAT.NexMotion_Define.DEV_TYPE_ETHERCAT);
            DaemonVars.stdout_stream.AutoFlush = true;

            // Start web
            Console.WriteLine("Start Web");
            Thread th = new Thread(new ThreadStart(StartWeb));
            th.Start();

            // Start Stdio
            Console.WriteLine("Start Stdio");
            Stdio.StartHandle(DaemonVars.router, DaemonVars.stdin_stream, DaemonVars.stdout_stream);

            // Shutdown web
            Console.WriteLine("Shutdown web");
            th.Interrupt();

            // Shutdown system
            Console.WriteLine("Shutdown system");
            DaemonVars.router.Shudown();

            Console.WriteLine("End Web");
        }

        public static void StartWeb()
        {
            IWebHost web_instance = WebHost.CreateDefaultBuilder(null)
                .UseStartup<Startup>()
                .Build();
            web_instance.Run();                
        }
    }
}
