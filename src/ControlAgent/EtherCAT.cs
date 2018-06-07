using System;
using System.Collections.Generic;
using NEXCOMROBOT.MCAT;

namespace NEXCOMROBOT
{
    public class EtherCAT
    {
        private List<RobotAgent> robots;
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

        public void InitIOForRobot(uint offset, int group_id)
        {
            if (robots.Count < 1)
            {
                Console.WriteLine("No such group to be assign to!");
                return;
            }
            
            if (offset > SystemConfig.NEXCOMROBOT_MAX_IO)
            {
                Console.WriteLine("Exceed pre-defined io range!");
                return;
            }

            NexMotion_IOAdapter tmp_IOAdapter = new NexMotion_IOAdapter(mRobot.DeviceID);
            GripperController tmp_gripper_ctl = new GripperController(tmp_IOAdapter, offset, offset);

            robots[group_id].SetGripper(tmp_gripper_ctl);
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
