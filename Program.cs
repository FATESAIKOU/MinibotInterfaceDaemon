using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEXCOMROBOT.MCAT;

//namespace work
namespace NEXCOMROBOT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            RobotAgent test_agent = new RobotAgent();
            // test_agent.SetMode(NexMotion_Define.DEV_TYPE_ETHERCAT);
            test_agent.Init();
            Console.WriteLine(test_agent.GetGroupStatus(0));
            test_agent.EnableGroup(0);
            Console.WriteLine(test_agent.GetGroupStatus(0));
            test_agent.DisableGroup(0);
            Console.WriteLine(test_agent.GetGroupStatus(0));
            test_agent.ResetGroup(0);
            Console.WriteLine(test_agent.GetGroupStatus(0));

            // Motion test
            test_agent.EnableGroup(0);
            test_agent.WaitStatus(0, 576, 100);

            Pos_T some_pos = new Pos_T();
            some_pos.pos = new double[]{90.0, 90.0, 90.0, 90.0, 90.0, 90.0};
            test_agent.SetHomePos(0, some_pos);
            test_agent.SetVelocity(0, some_pos.pos);
            
            Console.WriteLine("Start Homing!");
            Console.WriteLine(test_agent.GetGroupStatus(0));
            test_agent.HomeAll(0);
            test_agent.WaitStatus(0, 576, 100);
            Console.WriteLine("End Homing!");
            Console.WriteLine(test_agent.GetGroupStatus(0));

            Console.WriteLine("Start AcsJog!");
            Console.WriteLine(test_agent.GetGroupStatus(0));
            test_agent.AcsJog(0, 0, 0, 1000, 20);
            test_agent.WaitStatus(0, 576, 100);
            Console.WriteLine("End AcsJog!");
            Console.WriteLine(test_agent.GetGroupStatus(0));

            Console.WriteLine("Start PcsLine!");
            Console.WriteLine(test_agent.GetGroupStatus(0));
            some_pos.pos = new double[]{330, 10, -6, 0, 0, -180};
            Console.WriteLine(test_agent.PcsLine(0, some_pos, 0));
            test_agent.WaitStatus(0, 576, 100);
            Console.WriteLine("End PcsLine!");
            Console.WriteLine(test_agent.GetGroupStatus(0));

            Console.WriteLine("Start AcsPTP!");
            Console.WriteLine(test_agent.GetGroupStatus(0));
            some_pos.pos = new double[]{90, 90, 90, 90, 90, 90};
            Console.WriteLine(test_agent.AcsPTP(0, some_pos));
            test_agent.WaitStatus(0, 576, 100);
            Console.WriteLine("End AcsPTP!");
            Console.WriteLine(test_agent.GetGroupStatus(0));

            Console.WriteLine("Start PcsPTP!");
            Console.WriteLine(test_agent.GetGroupStatus(0));
            some_pos.pos = new double[]{93, 9, 821, 87, -0.1, 106};
            Console.WriteLine(test_agent.PcsPTP(0, some_pos));
            test_agent.WaitStatus(0, 576, 100);
            Console.WriteLine("End PcsPTP!");
            Console.WriteLine(test_agent.GetGroupStatus(0));

            test_agent.Shutdown();        
        }
    }
}
