using System;
using System.Collections.Generic;
using NEXCOMROBOT;

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
            return robot_agent.GetStatus();
        }

        static private RobotStatus Disable(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus Reset(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }
        #endregion        
        #region Initialize
        static private RobotStatus Home(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus HomeAll(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }
        #endregion
        #region RobotMove
        static private RobotStatus AcsJog(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus PcsLine(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus AcsPTP(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus PcsPTP(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }
        #endregion
        #region GripperMove
        static private RobotStatus Release(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus Grip(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }
        #endregion
        #region Wait
        static private RobotStatus Wait(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }

        static private RobotStatus WaitGripper(RobotAgent robot_agent, object[] args)
        {
            return robot_agent.GetStatus();
        }
        #endregion
    }
}