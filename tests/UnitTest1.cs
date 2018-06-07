using NUnit.Framework;
using System;
using NEXCOMROBOT;
using NEXCOMROBOT.MCAT;

namespace Tests
{
    public class Tests
    {
        private EtherCAT ether_cat_net;
        private int DeviceID;
        [SetUp]
        public void Setup()
        {
            ether_cat_net = new EtherCAT();
            ether_cat_net.InitRobot();
            ether_cat_net.InitIOForRobot(2, 0);
            DeviceID = 0;
        }

        [Test]
        public void StateTest()
        {
            RobotStatus robot_status;
            RobotAgent robot_agent = ether_cat_net.GetRobotAgent(DeviceID);

            robot_agent.Enable();
            robot_status = robot_agent.GetStatus();
            Assert.IsTrue(robot_status.state == 1 && robot_status.status == StateMaps.GetRobotStatusCode(
                new string[]{"ALL_STAND_STILL", "NO_POS_CHG"}
            ));
        }

        [Test]
        public void HomeTest()
        {
            Assert.Pass();
        }
    }
}