using System;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;
using Controller;

namespace DaemonCore
{
    public class Route
    {
        private EtherCAT ether_cat_net;

        public Route(int mode = NexMotion_Define.DEV_TYPE_SIMULATION)
        {
            ether_cat_net = new EtherCAT();
            ether_cat_net.SetMode(mode);

            int ret = ether_cat_net.InitRobot();
            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS)
                throw new System.SystemException("Unable to openup device(" + ret.ToString() + ")!!");

            ether_cat_net.InitIOForRobot(2, 0);
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