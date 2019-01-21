using System;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;
using LogicController;

namespace DaemonCore
{
    public class Route
    {
        public EtherCAT ether_cat_net;

        public void Start(int mode = NexMotion_Define.DEV_TYPE_SIMULATION)
        {
            ether_cat_net = new EtherCAT();
            ether_cat_net.SetMode(mode);

            int ret = ether_cat_net.InitRobot();
            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS)
                throw new System.SystemException("Unable to openup device(" + ret.ToString() + ")!!");

            ether_cat_net.InitIOForRobot(2, 0);
        }
        
        public ResponseStatus DoRoute(string target, string action, object[] args)
        {
            switch (target)
            {
                case "Robot":
                    RobotAgent robot_agent = ether_cat_net.GetRobotAgent(0);
                    return RobotController.Do(robot_agent, action, args);
                case "Camera":
                    // EzCameraController;
                    return EzCameraController.Do(action, args);
                default:
                    throw new System.ArgumentException("No Such Target: ", target);
            }
        }

        public void Shudown()
        {
            ether_cat_net.Shutdown();
        }
    }
}