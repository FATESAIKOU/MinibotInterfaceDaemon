using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEXCOMROBOT.MCAT;

namespace NEXCOMROBOT
{
    class EtherCAT
    {
        public List<RobotAgent> robots;
        private SystemControl mRobot = new SystemControl();
        private int OperatingMode = NexMotion_Define.DEV_TYPE_SIMULATION;

        public void SetMode(int type)
        {
            this.OperatingMode = type;
        }

        public void InitRobot()
        {
            mRobot.InitDevice(OperatingMode);

            robots = new List<RobotAgent>();

            RobotAgent tmp_agent;
            foreach (GroupControl g in mRobot.Group)
            {
                if (g.GroupAdapter == null || g.GroupParameters == null)
                {
                    Console.WriteLine("Null Group Detected!");
                }
                else
                {
                    tmp_agent = new RobotAgent(g);
                    tmp_agent.SysParamUpdate();
                    robots.Add(tmp_agent);
                }
            }
        }

        public void Shutdown()
        {
            mRobot.DeviceAdapter.NMC_DeviceShutdown();
        }

        public RobotAgent GetRobotAgent(int group_id)
        {
            return robots[group_id];
        }
    }
}
