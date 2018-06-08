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
                return ret;

            ether_cat_net.InitIOForRobot(2, 0);

            return ret;
        }
        
        public RobotStatus DoRoute(string target, string action, object[] params)
        {
            switch (target)
            {
                case "Robot":
                    // RobotController;
                    return RobotController.Do(action, params);
                case "Camera":
                    // CameraController;
                    return null;
                default:
                    throw new System.ArgumentException("No Such Action!!", "target");
            }
        }
    }
}