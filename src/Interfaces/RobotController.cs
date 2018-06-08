using System;
using System.Collections.Generic;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;

namespace Controller
{
    static public class RobotController
    {
        readonly static public Dictionary<string, Func<RobotAgent, object[], RobotStatus>> selector = new Dictionary<string, Func<RobotAgent, object[], RobotStatus>>()
        {
            { "Enable", Enable }, { "Disable", Disable }, { "Reset", Reset },
            { "Home", Home }, {"HomeAll", HomeAll},
            { "AcsJog", AcsJog }, { "PcsLine", PcsLine }, { "AcsPTP", AcsPTP }, { "PcsPTP", PcsPTP },
            { "Release", Release }, { "Grip", Grip },
            { "Wait", Wait }, { "WaitGripper", WaitGripper }
        };

        static private int MAX_VEL = 20;
        static private int WAIT_TIMEOUT = 3000;

        static public RobotStatus Do(RobotAgent robot_agent, string action, object[] args)
        {
            Func<RobotAgent, object[], RobotStatus> act_func;
            if (selector.TryGetValue(action, out act_func))
            {
                return act_func(robot_agent, args);
            }
            else
            {
                throw new System.ArgumentException("No Such Action!!", "action");
            }
        }


        // Private process
        #region StateControl
        static private RobotStatus Enable(RobotAgent robot_agent, object[] args)
        {
            robot_agent.Enable();
            return robot_agent.GetStatus();
        }

        static private RobotStatus Disable(RobotAgent robot_agent, object[] args)
        {
            robot_agent.Disable();
            return robot_agent.GetStatus();
        }

        static private RobotStatus Reset(RobotAgent robot_agent, object[] args)
        {
            robot_agent.Reset();
            return robot_agent.GetStatus();
        }
        #endregion
        #region Initialize
        static private RobotStatus Home(RobotAgent robot_agent, object[] args)
        {
            if ( (int)args[0] < 0 || 5 < (int)args[0] )
                throw new System.ArgumentOutOfRangeException("AxisId out of range!!", "AxisId(args[0])");

            robot_agent.HomeRobot( (int)args[0] );

            return robot_agent.GetStatus();
        }

        static private RobotStatus HomeAll(RobotAgent robot_agent, object[] args)
        {
            robot_agent.HomeAll();

            return robot_agent.GetStatus();
        }
        #endregion
        #region RobotMove
        static private RobotStatus AcsJog(RobotAgent robot_agent, object[] args)
        {            
            if ( (int)args[0] < 0 || 5 < (int)args[0] )
                throw new System.ArgumentOutOfRangeException("AxisId out of range!!", "AxisId(args[0])");

            if ( (int)args[1] != 0 && (int)args[1] != 1 )
                throw new System.NotSupportedException("Not supported direction!!: Dir(args[1])");

            if ( (int)args[2] < 0 || 2000 < (int)args[2] )
                throw new System.ArgumentOutOfRangeException("Interval out of range!!", "Interval(args[2])");

            robot_agent.AcsJog((int)args[0], (int)args[1], (int)args[2], MAX_VEL);

            return robot_agent.GetStatus();
        }

        static private RobotStatus PcsLine(RobotAgent robot_agent, object[] args)
        {
            Pos_T dest = new Pos_T();
            dest.pos = (double[])args[0];
            
            robot_agent.PcsLine(dest, MAX_VEL);

            return robot_agent.GetStatus();
        }

        static private RobotStatus AcsPTP(RobotAgent robot_agent, object[] args)
        {
            Pos_T dest = new Pos_T();
            dest.pos = (double[])args[0];
            
            robot_agent.AcsPTP(dest);

            return robot_agent.GetStatus();
        }

        static private RobotStatus PcsPTP(RobotAgent robot_agent, object[] args)
        {
            Pos_T dest = new Pos_T();
            dest.pos = (double[])args[0];
            
            robot_agent.PcsPTP(dest);

            return robot_agent.GetStatus();
        }
        #endregion
        #region GripperMove
        static private RobotStatus Release(RobotAgent robot_agent, object[] args)
        {
            robot_agent.ReleaseGripper();
            return robot_agent.GetStatus();
        }

        static private RobotStatus Grip(RobotAgent robot_agent, object[] args)
        {
            robot_agent.MoveGripper((ushort)args[0], (ushort)args[1]);
            return robot_agent.GetStatus();
        }
        #endregion
        #region Wait
        static private RobotStatus Wait(RobotAgent robot_agent, object[] args)
        {
            robot_agent.WaitStatus((int)args[0], (int)args[1], WAIT_TIMEOUT);
            return robot_agent.GetStatus();
        }

        static private RobotStatus WaitGripper(RobotAgent robot_agent, object[] args)
        {
            robot_agent.WaitGripperBusy((int)args[0], WAIT_TIMEOUT);
            return robot_agent.GetStatus();
        }
        #endregion
    }
}