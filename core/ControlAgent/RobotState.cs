using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NEXCOMROBOT
{
    static public class StateMaps
    {
        readonly static public string[] RobotStateMap = new string[] {
            "GROUP_DISABLE",
            "GROUP_STAND_STILL",
            "GROUP_STOPPED",
            "GROUP_STOPPING",
            "GROUP_MOVING",
            "GROUP_HOMING",
            "GROUP_ERROR_STOP"
        };

        readonly static public string[] RobotStatusMap = new string[] {
            "OUTER_EMGSTOP",
            "DRIVER_ALARM",
            "HW_PLIM_EXCEED",
            "HW_NLIM_EXCEED",
            "SW_PLIM_EXCEED",
            "SW_NLIM_EXCEED",
            "ALL_STAND_STILL",
            "GROUP_ERR_STOP",
            "",
            "NO_POS_CHG",
            "PCS_ACC_MOV",
            "PCS_DCC_MOV",
            "PCS_MAX_MOV",
            "GROUP_MOVING",
            "GROUP_STOPPED"
        };

        public static int GetRobotStatusCode(string[] status)
        {
            int code = 0;
            int ind = 0;
            foreach(string s in status)
            {
                ind = Array.IndexOf(RobotStatusMap, s);
                if (ind == -1) return -1;

                code |= 1 << ind;
            }

            return code;
        }

    }
    public class RobotStatus
    {
        public int state;
        public int status;
        public double[] acs;
        public double[] pcs;
        public GripperStatus gripper_status;
        public int ret_code;

        public string DumpJson()
        {
            return JsonConvert.SerializeObject(this.DoMap(), Formatting.Indented);
        }

        public RobotStatusReadable DoMap()
        {
            List<string> tmp_status = new List<string>();
            for (int i = 0; i < 15; i ++)
            {
                if ( (this.status & (1 << i)) > 0 ) {
                    tmp_status.Add(StateMaps.RobotStatusMap[i]);
                }
            }

            RobotStatusReadable robot_status_readable = new RobotStatusReadable();
            robot_status_readable.state = StateMaps.RobotStateMap[this.state];
            robot_status_readable.status = tmp_status.ToArray();
            robot_status_readable.acs = this.acs;
            robot_status_readable.pcs = this.pcs;
            robot_status_readable.ret_code = this.ret_code;
            
            if (this.gripper_status != null)
                robot_status_readable.gripper_status_readable = this.gripper_status.DoMap();

            return robot_status_readable;
        }
    }

    public class RobotStatusReadable
    {
        public string state;
        public string[] status;
        public double[] acs;
        public double[] pcs;
        public int ret_code;
        public GripperStatusReadable gripper_status_readable;
    }

    public class GripperStatus
    {
        public bool is_ready;
        public bool is_busy;
        public bool is_gripped;
        public double current_pos;
        public int alarm_code;

        public string DumpJson()
        {
            return JsonConvert.SerializeObject(this.DoMap(), Formatting.Indented);
        }

        public GripperStatusReadable DoMap()
        {
            GripperStatusReadable gripper_status_readable = new GripperStatusReadable();
            gripper_status_readable.is_ready = this.is_ready;
            gripper_status_readable.is_busy = this.is_busy;
            gripper_status_readable.is_gripped = this.is_gripped;
            gripper_status_readable.current_pos = this.current_pos;
            gripper_status_readable.alarm_code = this.alarm_code;

            return gripper_status_readable;
        }
    }

    public class GripperStatusReadable
    {
        public bool is_ready;
        public bool is_busy;
        public bool is_gripped;
        public double current_pos;
        public int alarm_code; // TODO: Make it readable.    
    }
}