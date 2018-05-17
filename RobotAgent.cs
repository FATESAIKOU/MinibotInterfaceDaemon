using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEXCOMROBOT.MCAT;

//namespace work
namespace NEXCOMROBOT
{
    class RobotAgent
    {
        private SystemControl mRobot = new SystemControl();
        private int OperatingMode = NexMotion_Define.DEV_TYPE_SIMULATION;

        /* State Setting */
        public void Init()
        {
            mRobot.InitDevice(OperatingMode);

            foreach (GroupControl g in mRobot.Group)
            {
                if (g.GroupAdapter == null || g.GroupParameters == null)
                {
                    Console.WriteLine("Null Group Detected!");
                }
            }
            SysParamUpdate();
        }

        public void Shutdown()
        {
            mRobot.DeviceAdapter.NMC_DeviceShutdown();
        }

        public void SetMode(int type)
        {
            this.OperatingMode = type;
        }

        public void EnableGroup(int group_id)
        {
            mRobot.Group[group_id].GroupAdapter.NMC_GroupEnable();
        }

        public void DisableGroup(int group_id)
        {
            mRobot.Group[group_id].GroupAdapter.NMC_GroupDisable();
        }

        public void ResetGroup(int group_id)
        {
            mRobot.Group[group_id].GroupAdapter.NMC_GroupDisable();
            mRobot.Group[group_id].GroupAdapter.NMC_GroupResetState();
        }
        
        /* State Getting */
        public string GetGroupStatus(int group_id)
        {
            SysParamUpdate();

            var group_parameters = mRobot.Group[group_id].GroupParameters;
            
            string status_string = "";
            bool hf = false;
            for (int i = 0; i < 15; i ++) {
                if ( (group_parameters.Status & (1 << i)) > 0 ) {
                    status_string += '"' + StatusMap[i] + "\",";
                    hf = true;
                }
            }
            if (hf) {
                status_string = status_string.Substring(0, status_string.Length - 1);
            }

            string json_result = "{";
            json_result += "\"State\": \"" + StateMap[group_parameters.State] + "\",\n";
            json_result += "\"Status\": [" + status_string + "],\n";            
            json_result += "\"Acs\": [" + string.Join(",", group_parameters.ActAcs) + "],\n";
            json_result += "\"Pcs\": [" + string.Join(",", group_parameters.ActPcs) + "]\n";
            json_result += "}";

            return json_result;
        }

        public void WaitStatus(int group_id, int status_mask, int interval)
        {
            GroupControl aim_group = mRobot.Group[group_id];

            int status = 0;
            do {
                aim_group.GroupAdapter.NMC_GroupGetStatus(ref status);
                System.Threading.Thread.Sleep(interval);
            } while((status ^ status_mask) != 0);
        }

        /* Param Setting */
        public void SetHomePos(int group_id, Pos_T home_pos)
        {
            int mask = 1 << mRobot.Group[group_id].AxisCount - 1;

            mRobot.Group[group_id].GroupAdapter.NMC_GroupSetHomePos(mask, ref home_pos);
            SysParamUpdate();
        }

        public void SetVelocity(int group_id, double[] vel)
        {
            int param_num = 0x32;
            int sub_index = 0;

            GroupControl aim_group = mRobot.Group[group_id];
            for (int axis_index = 0; axis_index < aim_group.AxisCount; ++ axis_index)
            {
                aim_group.GroupAdapter.NMC_GroupAxSetParamF64(
                    axis_index, param_num, sub_index,
                    vel[axis_index]
                );
            }
            SysParamUpdate();
        }
        
        /* Moving */
        public int Halt(int group_id)
        {
            return mRobot.Group[group_id].GroupAdapter.NMC_GroupHalt();
        }

        public int EMGStop(int group_id)
        {
            return mRobot.Group[group_id].GroupAdapter.NMC_GroupStop();
        }

        public int Home(GroupControl aim_group, int axis_index)
        {
            int param_num = 0x80;
            int sub_index = 0;
            int ret;

            ret = aim_group.GroupAdapter.NMC_GroupAxSetParamI32(
                axis_index, param_num, sub_index,
                HomingOpcodes[Math.Min(axis_index, 6)]
            );

            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) return ret;

            ret = aim_group.GroupAdapter.NMC_GroupAxesHomeDrive(1 << axis_index);

            return ret;
        }

        public int HomeAll(int group_id)
        {
            int ret = 0;

            GroupControl aim_group = mRobot.Group[group_id];
            for (int axis_index = 0; axis_index < aim_group.AxisCount; ++ axis_index)
            {
                ret = Home(aim_group, axis_index);
                if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) break;
            }

            return ret;
        }

        // Add Line, Circle, Jog for Acs/Pcs interface
        public int AcsJog(int group_id, int axis_index, int direction, int interval,
            double max_vel /* ohn... no zero */)
        {
            GroupControl aim_group = mRobot.Group[group_id];

            int ret;
            ret = aim_group.GroupAdapter.NMC_GroupJogAcs(axis_index, direction, ref max_vel);
            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) return ret;

            System.Threading.Thread.Sleep(interval);
            ret = aim_group.GroupAdapter.NMC_GroupHalt();
            return ret;
        }

        public int PcsLine(int group_id, Pos_T dest,
            double max_vel /* ohn... it can be zero this time... */)
        {
            GroupControl aim_group = mRobot.Group[group_id];
            int mask = (1 << aim_group.AxisCount) - 1;

            return aim_group.GroupAdapter.NMC_GroupLine(mask, ref dest, ref max_vel);
        }

        public int AcsPTP(int group_id, Pos_T dest_acs)
        {
            GroupControl aim_group = mRobot.Group[group_id];
            int mask = (1 << aim_group.AxisCount) - 1;

            return aim_group.GroupAdapter.NMC_GroupPtpAcsAll(mask, ref dest_acs);
        }

        public int PcsPTP(int group_id, Pos_T dest)
        {
            GroupControl aim_group = mRobot.Group[group_id];
            int mask = (1 << aim_group.AxisCount) - 1;

            return aim_group.GroupAdapter.NMC_GroupPtpCartAll(mask, ref dest);
        }

        /* Other Utils */
        private int SysParamUpdate()
        {
            NexMotion_GroupAdapter groupAdapter;
            RobotParameters groupParameters;

            int axisCount;

            #region Group
            foreach (GroupControl g in mRobot.Group)
            {
                groupAdapter    = g.GroupAdapter;
                groupParameters = g.GroupParameters;
                axisCount       = g.AxisCount;

                groupAdapter.NMC_GroupGetState(ref groupParameters.State);
                groupAdapter.NMC_GroupGetStatus(ref groupParameters.Status);
                groupAdapter.NMC_GroupGetActualPosAcs(ref groupParameters.PosAcs);
                groupAdapter.NMC_GroupGetActualPosPcs(ref groupParameters.PosPcs);

                // WTF...
                groupParameters.ActAcs = groupParameters.PosAcs.pos;
                groupParameters.ActPcs = groupParameters.PosPcs.pos;
            }
            #endregion
            
            /* TODO: IO Sync? */

            return NexMotion_ErrCode.NMCERR_SUCCESS;
        }

        /* Decoder */
        private string[] StateMap = new string[] {
            "GROUP_DISABLE",
            "GROUP_STAND_STILL",
            "GROUP_STOPPED",
            "GROUP_STOPPING",
            "GROUP_MOVING",
            "GROUP_HOMING",
            "GROUP_ERROR_STOP"
        };

        private string[] StatusMap = new string[] {
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

        private int[] HomingOpcodes = new int[] {
            20, 20, 22, 20, 22, 20,
            35 // Default is 35
        };
    }
}
