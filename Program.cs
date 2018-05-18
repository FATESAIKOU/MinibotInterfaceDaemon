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
            
            EtherCAT ether_cat_net = new EtherCAT();
            //ether_cat_net.SetMode(NexMotion_Define.DEV_TYPE_ETHERCAT);

            ether_cat_net.InitRobot();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Enable();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Disable();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Reset();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            // Motion test
            ether_cat_net.GetRobotAgent(0).Enable();
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);

            Pos_T some_pos = new Pos_T();
            some_pos.pos = new double[]{90.0, 90.0, 90.0, 90.0, 90.0, 90.0};
            ether_cat_net.GetRobotAgent(0).SetHomePos(some_pos);
            ether_cat_net.GetRobotAgent(0).SetVelocity(some_pos.pos);
            
            Console.WriteLine("Start Homing!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).HomeAll();
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("End Homing!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            Console.WriteLine("Start AcsJog!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).AcsJog(0, 0, 1000, 20);
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("End AcsJog!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            Console.WriteLine("Start PcsLine!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            some_pos.pos = new double[]{330, 10, -6, 0, 0, -180};
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).PcsLine(some_pos, 0));
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("End PcsLine!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            Console.WriteLine("Start AcsPTP!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            some_pos.pos = new double[]{90, 90, 90, 90, 90, 90};
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).AcsPTP(some_pos));
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("End AcsPTP!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            Console.WriteLine("Start PcsPTP!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            some_pos.pos = new double[]{93, 9, 821, 87, -0.1, 106};
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).PcsPTP(some_pos));
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("End PcsPTP!");
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());

            ether_cat_net.Shutdown();        
        }
    }
}
