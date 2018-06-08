using System;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;
using Controller;

namespace DaemonCore
{
    public class Route
    {
        EtherCAT ether_cat_net;

        public int EnableRoute()
        {
            int ret = NexMotion_ErrCode.NMCERR_SUCCESS;

            ether_cat_net = new EtherCAT();
            ret = ether_cat_net.InitRobot();

            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS)
                throw new System.SystemException("Unable to openup device(" + ret.ToString() + ")!!");

            ether_cat_net.InitIOForRobot(2, 0);

            return ret;
        }
        
        public RobotStatus DoRoute(string target, string action, object[] args)
        {
            switch (target)
            {
                case "Robot":
                    RobotAgent robot_agent = ether_cat_net.GetRobotAgent(0);
                    return RobotController.Do(robot_agent, action, args);
                case "Camera":
                    // CameraController;
                    return null;
                default:
                    throw new System.ArgumentException("No Such Target!!", "target");
            }
        }
    }
}