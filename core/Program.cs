using System;
using System.IO;
using System.Text.RegularExpressions;
using DaemonCore;
using DaemonInterface;
using NEXCOMROBOT.MCAT;

//namespace work
namespace NEXCOMROBOT
{
    public static class DaemonVars
    {
        public static Route router = new Route();
        public static StreamReader stdin_stream = new System.IO.StreamReader(Console.OpenStandardInput());
        public static StreamWriter stdout_stream = new System.IO.StreamWriter(Console.OpenStandardOutput());

    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World");

            // Init router
            DaemonVars.router.Start();
            //DaemonVars.router.Start(NexMotion_Define.DEV_TYPE_ETHERCAT);

            // Init stdin/stdout
            DaemonVars.stdout_stream.AutoFlush = true;

            // Daemon handler
            DaemonHandler();

            // Shutdown router
            DaemonVars.router.Shudown();

            Console.WriteLine("Bye, World");
        }

        static void DaemonHandler()
        {
            StreamReader src = DaemonVars.stdin_stream;
            StreamWriter dest = DaemonVars.stdout_stream;
            string command;

            while (true)
            {
                dest.Write("[Daemon-cmd]: ");
                command = src.ReadLine();
                switch (command)
                {
                    case "StdioStart":
                        Stdio.StartHandle(DaemonVars.router, DaemonVars.stdin_stream, DaemonVars.stdout_stream);
                        break;
                    case "WebStart":
                        break;
                    case "WebShutdown":
                        break;
                    case "Shutdown":
                        return;
                    default:
                        Console.WriteLine("[Daemon-ret]: invalid command format.");
                        break;
                }
            }
        }
    }
}
