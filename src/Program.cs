using System;
using System.IO;
using DaemonCore;
using DaemonInterface;
using NEXCOMROBOT.MCAT;

//namespace work
namespace NEXCOMROBOT
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World");

            
            Route router = new Route();
            router.Start();
            //router.Start(NexMotion_Define.DEV_TYPE_ETHERCAT);

            StreamReader curr_src = new System.IO.StreamReader(Console.OpenStandardInput());
            StreamWriter curr_dest = new System.IO.StreamWriter(Console.OpenStandardOutput());
            curr_dest.AutoFlush = true;
            Stdio.StartHandle(router, curr_src, curr_dest);

            router.Shudown();

            Console.WriteLine("Bye, World");
        }

        static void StateTest(Route router)
        {
            Console.WriteLine( "[Enable]" + router.DoRoute("Robot", "Enable", null).DumpJson() );
            Console.WriteLine( "[Disable]" + router.DoRoute("Robot", "Disable", null).DumpJson() );
            Console.WriteLine( "[Enable]" + router.DoRoute("Robot", "Enable", null).DumpJson() );
            Console.WriteLine( "[Reset]" + router.DoRoute("Robot", "Reset", null).DumpJson() );
        }

        static void MotionTest(Route router)
        {
            Console.WriteLine( "[Enable]" + router.DoRoute("Robot", "Enable", null).DumpJson() );

            Console.WriteLine( "[Home]" + router.DoRoute("Robot", "Home", new object[]{0}).DumpJson() );
            Console.WriteLine( "[Home-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            Console.WriteLine( "[HomeAll]" + router.DoRoute("Robot", "HomeAll", null).DumpJson() );
            Console.WriteLine( "[HomeAll-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            
            Console.WriteLine( "[AcsJog]" + router.DoRoute("Robot", "AcsJog", new object[]{0, 0, 1000}).DumpJson() );
            Console.WriteLine( "[AcsJog-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            
            Console.WriteLine( "[PcsLine]" + router.DoRoute("Robot", "PcsLine", new object[]{new double[]{330, 10, -6, 0, 0, -180}}).DumpJson() );
            Console.WriteLine( "[PcsLine-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            Console.WriteLine( "[AcsPTP]" + router.DoRoute("Robot", "AcsPTP", new object[]{new double[]{90, 90, 90, 90, 90, 90}}).DumpJson() );
            Console.WriteLine( "[AcsPTP-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            Console.WriteLine( "[PcsPTP]" + router.DoRoute("Robot", "PcsPTP", new object[]{new double[]{93, 9, 821, 87, -0.1, 106}}).DumpJson() );
            Console.WriteLine( "[PcsPTP-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "" );
        }

        static void GripperTest(Route router)
        {
            Console.WriteLine( "[Enable]" + router.DoRoute("Robot", "Enable", null).DumpJson() );

            Console.WriteLine( "[Home]" + router.DoRoute("Robot", "Home", new object[]{5}).DumpJson() );
            Console.WriteLine( "[Home-End]" + router.DoRoute("Robot", "Wait", new object[]{576, 100}).DumpJson() );
            Console.WriteLine( "\n" );

            Console.WriteLine( "[HomeGripper]" );
            router.ether_cat_net.GetRobotAgent(0).HomeGripper(); // No Such Interface;
            Console.WriteLine( "[HomeGripper-End]" + router.DoRoute("Robot", "WaitGripper", new object[]{100}).DumpJson() );
            Console.WriteLine( "\n" );

            double[] some_pos = router.ether_cat_net.GetRobotAgent(0).group_ctrl.GroupParameters.ActAcs;
            Console.WriteLine(some_pos);

            Console.WriteLine("\n[TEST]\n" + router.ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());

            RobotStatus now_state;
            for (int i = 0; i < 0; i ++)
            {
                some_pos[5] = -180;
                router.DoRoute("Robot", "AcsPTP", new object[]{some_pos});
                router.DoRoute("Robot", "Release", null);
                router.DoRoute("Robot", "WaitGripper", new object[]{100});
                now_state = router.DoRoute("Robot", "Wait", new object[]{576, 100});
                Console.WriteLine("\n[ReleaseGripper(" + i.ToString() + ")]\n" + now_state.DumpJson());

                some_pos[5] = 0;
                router.DoRoute("Robot", "AcsPTP", new object[]{some_pos});
                router.DoRoute("Robot", "Grip", new object[]{(ushort)50, (ushort)40});
                router.DoRoute("Robot", "WaitGripper", new object[]{100});
                now_state = router.DoRoute("Robot", "Wait", new object[]{576, 100});
                Console.WriteLine("\n[MoveGripper(" + i.ToString() + ")]\n" + now_state.DumpJson());
            }            
        }

        static void StateTest(EtherCAT ether_cat_net)
        {
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Enable();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Disable();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());
            ether_cat_net.GetRobotAgent(0).Reset();
            Console.WriteLine(ether_cat_net.GetRobotAgent(0).GetStatus());            
        }

        static void MotionTest(EtherCAT ether_cat_net)
        {
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
        }

        static void GripperTest(EtherCAT ether_cat_net)
        {
            ether_cat_net.GetRobotAgent(0).Enable();
            Console.WriteLine("\n[Enable]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());
            ether_cat_net.GetRobotAgent(0).Disable();
            Console.WriteLine("\n[Disable]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());
            ether_cat_net.GetRobotAgent(0).Reset();
            Console.WriteLine("\n[Reset]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());

            ether_cat_net.GetRobotAgent(0).Enable();
            Console.WriteLine("\n[Enable]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());

            ether_cat_net.GetRobotAgent(0).HomeRobot(5);
            ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
            Console.WriteLine("\n[Start]\n" + ether_cat_net.GetRobotAgent(0).GetStatus().DumpJson());

            ether_cat_net.GetRobotAgent(0).HomeGripper();            
            ether_cat_net.GetRobotAgent(0).WaitGripperBusy();
            Console.WriteLine("\n[Home]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());

            Pos_T some_pos = new Pos_T();
            some_pos.pos = ether_cat_net.GetRobotAgent(0).group_ctrl.GroupParameters.ActAcs;
            Console.WriteLine(some_pos.pos[0]);

            Console.WriteLine("\n[TEST]\n" + ether_cat_net.GetRobotAgent(0).GetGripperStatus().DumpJson());
            for (int i = 0; i < 0; i ++)
            {
                some_pos.pos[5] = -180;
                ether_cat_net.GetRobotAgent(0).AcsPTP(some_pos);
                ether_cat_net.GetRobotAgent(0).ReleaseGripper();
                ether_cat_net.GetRobotAgent(0).WaitGripperBusy();
                ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
                Console.WriteLine("\n[ReleaseGripper(" + i.ToString() + ")]\n" + ether_cat_net.GetRobotAgent(0).GetStatus().DumpJson());

                some_pos.pos[5] = 0;
                ether_cat_net.GetRobotAgent(0).AcsPTP(some_pos);
                ether_cat_net.GetRobotAgent(0).MoveGripper(50, 40);
                ether_cat_net.GetRobotAgent(0).WaitGripperBusy();
                ether_cat_net.GetRobotAgent(0).WaitStatus(576, 100);
                Console.WriteLine("\n[MoveGripper(" + i.ToString() + ")]\n" + ether_cat_net.GetRobotAgent(0).GetStatus().DumpJson());
            }

        }
    }
}
