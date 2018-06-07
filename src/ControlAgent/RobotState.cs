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

    }
    public class RobotState
    {
        public int state;
        public int status;
        public double[] acs;
        public double[] pcs;
        public GripperState gripper_state;

        public string DumpJson()
        {
            return JsonConvert.SerializeObject(this.DoMap(), Formatting.Indented);
        }

        public RobotStateReadable DoMap()
        {
            List<string> tmp_status = new List<string>();
            for (int i = 0; i < 15; i ++)
            {
                if ( (this.status & (1 << i)) > 0 ) {
                    tmp_status.Add(StateMaps.RobotStatusMap[i]);
                }
            }

            RobotStateReadable robot_state_readable = new RobotStateReadable();
            robot_state_readable.state = StateMaps.RobotStateMap[this.state];
            robot_state_readable.status = tmp_status.ToArray();
            robot_state_readable.acs = this.acs;
            robot_state_readable.pcs = this.pcs;
            robot_state_readable.gripper_state_readable = this.gripper_state.DoMap();

            return robot_state_readable;
        }
    }

    public class RobotStateReadable
    {
        public string state;
        public string[] status;
        public double[] acs;
        public double[] pcs;
        public GripperStateReadable gripper_state_readable;
    }

    public class GripperState
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

        public GripperStateReadable DoMap()
        {
            GripperStateReadable gripper_state_readable = new GripperStateReadable();
            gripper_state_readable.is_ready = this.is_ready;
            gripper_state_readable.is_busy = this.is_busy;
            gripper_state_readable.is_gripped = this.is_gripped;
            gripper_state_readable.current_pos = this.current_pos;
            gripper_state_readable.alarm_code = this.alarm_code;

            return gripper_state_readable;
        }
    }

    public class GripperStateReadable
    {
        public bool is_ready;
        public bool is_busy;
        public bool is_gripped;
        public double current_pos;
        public int alarm_code; // TODO: Make it readable.    
    }
}