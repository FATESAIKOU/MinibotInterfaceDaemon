using System;
using System.IO;
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

            // Stdio handler
            Stdio.StartHandle(DaemonVars.router, DaemonVars.stdin_stream, DaemonVars.stdout_stream);

            // Shutdown router
            DaemonVars.router.Shudown();

            Console.WriteLine("Bye, World");
        }
    }
}
