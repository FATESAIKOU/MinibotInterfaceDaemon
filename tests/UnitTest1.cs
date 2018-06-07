using NUnit.Framework;
using NEXCOMROBOT;

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

            DeviceID = 0;
        }

        [Test]
        public void StateTest()
        {
            Assert.Pass();
        }

        [Test]
        public void HomeTest()
        {
            Assert.Pass();
        }
    }
}