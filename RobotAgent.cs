using System;
using NEXCOMROBOT.MCAT;

namespace NEXCOMROBOT
{
    class RobotAgent
    {
        private GroupControl group_ctrl;
        public GripperController gripper_ctl;

        public RobotAgent(GroupControl group_ctrl)
        {
            this.group_ctrl = group_ctrl;
        }

        public void SetGripper(GripperController gripper_ctl)
        {
            this.gripper_ctl = gripper_ctl;
        }

        /* State Setting */
        #region StateSetting
        public void Enable()
        {
            group_ctrl.GroupAdapter.NMC_GroupEnable();
            EnableGripper();
        }

        public void Disable()
        {
            group_ctrl.GroupAdapter.NMC_GroupDisable();
            DisableGripper();
        }

        public void Reset()
        {
            group_ctrl.GroupAdapter.NMC_GroupDisable();
            group_ctrl.GroupAdapter.NMC_GroupResetState();
            DisableGripper();
            ResetGripper();
        }
        #endregion
        
        /* State Getting */
        #region StateGetting
        public string GetStatus()
        {
            SysParamUpdate();

            var group_parameters = group_ctrl.GroupParameters;
            
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
            json_result += "\"Gripper\":" + GetGripperStatus() + "\n";
            json_result += "}";

            return json_result;
        }
        #endregion

        /* All Moving */
        #region AllMoving
        public int Halt()
        {
            return group_ctrl.GroupAdapter.NMC_GroupHalt();
        }

        public int EMGStop()
        {
            return group_ctrl.GroupAdapter.NMC_GroupStop();
        }

        public int HomeAll()
        {
            if (CheckParam(AvalibleStatus) == false)
                return -1;
            
            int ret = 0;
            for (int axis_index = 0; axis_index < group_ctrl.AxisCount; ++ axis_index)
            {
                ret = HomeRobot(axis_index);
                if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) break;
            }

            HomeGripper();

            return ret;
        }
        #endregion

        /* Robot Param Setting */
        #region RobotParamSetting
        public void SetHomePos(Pos_T home_pos)
        {
            int mask = 1 << group_ctrl.AxisCount - 1;

            group_ctrl.GroupAdapter.NMC_GroupSetHomePos(mask, ref home_pos);
            SysParamUpdate();
        }

        public void SetVelocity(double[] vel)
        {
            int param_num = 0x32;
            int sub_index = 0;

            for (int axis_index = 0; axis_index < group_ctrl.AxisCount; ++ axis_index)
            {
                group_ctrl.GroupAdapter.NMC_GroupAxSetParamF64(
                    axis_index, param_num, sub_index,
                    vel[axis_index]
                );
            }
            SysParamUpdate();
        }
        #endregion
        
        /* Robot Moving */
        #region RobotMoving
        public int HomeRobot(int axis_index)
        {
            int param_num = 0x80;
            int sub_index = 0;
            int ret;

            ret = group_ctrl.GroupAdapter.NMC_GroupAxSetParamI32(
                axis_index, param_num, sub_index,
                HomingOpcodes[Math.Min(axis_index, 6)]
            );

            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) return ret;

            ret = group_ctrl.GroupAdapter.NMC_GroupAxesHomeDrive(1 << axis_index);

            return ret;
        }

        public int AcsJog(int axis_index, int direction, int interval,
            double max_vel /* ohn... no zero */)
        {
            if (CheckParam(AvalibleStatus) == false)
                return -1;

            int ret;
            ret = group_ctrl.GroupAdapter.NMC_GroupJogAcs(axis_index, direction, ref max_vel);
            if (ret != NexMotion_ErrCode.NMCERR_SUCCESS) return ret;

            System.Threading.Thread.Sleep(interval);
            ret = group_ctrl.GroupAdapter.NMC_GroupHalt();
            return ret;
        }

        // No circle interface
        public int PcsLine(Pos_T dest,
            double max_vel /* ohn... it can be zero this time... */)
        {
            if (CheckParam(AvalibleStatus) == false)
                return -1;

            int mask = (1 << group_ctrl.AxisCount) - 1;

            return group_ctrl.GroupAdapter.NMC_GroupLine(mask, ref dest, ref max_vel);
        }

        public int AcsPTP(Pos_T dest_acs)
        {
            if (CheckParam(AvalibleStatus) == false)
                return -1;

            int mask = (1 << group_ctrl.AxisCount) - 1;

            return group_ctrl.GroupAdapter.NMC_GroupPtpAcsAll(mask, ref dest_acs);
        }

        public int PcsPTP(Pos_T dest)
        {
            if (CheckParam(AvalibleStatus) == false)
                return -1;

            int mask = (1 << group_ctrl.AxisCount) - 1;

            return group_ctrl.GroupAdapter.NMC_GroupPtpCartAll(mask, ref dest);
        }
        #endregion

        /* Gripper State Setting */
        #region GripperStateSetting
        public void EnableGripper()
        {
            gripper_ctl.SVON = true;

            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
            } while (!gripper_ctl.SVRE);
        }

        public void DisableGripper()
        {
            gripper_ctl.SVON = false;

            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
            } while (gripper_ctl.SVON);
        }

        public void ResetGripper()
        {
            gripper_ctl.RESET = true;

            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
            } while (gripper_ctl.ALARM);

            gripper_ctl.RESET = false;
        }
        #endregion

        /* Gripper State Getting */
        #region GripperStateGetting
        public string GetGripperStatus()
        {
            bool   is_gripped = gripper_ctl.INP;
            double current_pos = gripper_ctl.Current_Position / 100.0;
            int    alarm_code = gripper_ctl.Alarm_1;
            bool   is_ready = gripper_ctl.Ready;
            bool   is_busy = gripper_ctl.BUSY;

            string status_string = "{\n";
            status_string += "\"IsGripped\": " + is_gripped.ToString() + ",\n";
            status_string += "\"CurrentPos\": " + current_pos.ToString() + ",\n";
            status_string += "\"AlarmCode\": " + alarm_code.ToString() + ",\n";
            status_string += "\"IsReady\": " + is_ready.ToString() + ",\n";
            status_string += "\"IsBusy\": " + is_busy.ToString() + ",\n";
            status_string += "}\n";

            return status_string;
        }
        #endregion

        /* Gripper Moving */
        #region GripperMoving
        public int HomeGripper()
        {
            if ( gripper_ctl.BUSY || !gripper_ctl.Ready || gripper_ctl.ALARM )
                return 1;

            gripper_ctl.SETUP = true;

            while (!gripper_ctl.BUSY && !gripper_ctl.ALARM)
                System.Threading.Thread.Sleep(gripper_check_interval);

            gripper_ctl.SETUP = false;
            
            return gripper_ctl.ALARM ? 1:0;
        }

        public int MoveGripper(ushort pushing_force, ushort trigger_LV)
        {
            if ( gripper_ctl.BUSY || !gripper_ctl.Ready || gripper_ctl.ALARM )
                return 1;

            gripper_ctl.DataFlag = 65535;
            gripper_ctl.MovementMode = 1;
            gripper_ctl.Positon = gripper_ctl.Current_Position - 10;
            gripper_ctl.Speed = 100;
            gripper_ctl.Acc = 100;
            gripper_ctl.Dec = 100;
            gripper_ctl.Pusing_Force = pushing_force;
            gripper_ctl.Trigger_LV = trigger_LV;
            gripper_ctl.Pushing_Speed = 10;
            gripper_ctl.Moving_Force = 100;
            gripper_ctl.Area_1 = 0;
            gripper_ctl.Area_2 = 0;
            gripper_ctl.In_Position = 5000;

            gripper_ctl.StartFlag = true;
            while (!gripper_ctl.BUSY && !gripper_ctl.ALARM)
                System.Threading.Thread.Sleep(gripper_check_interval);
            gripper_ctl.StartFlag = false;

            return gripper_ctl.ALARM ? 1:0;
        }

        public int ReleaseGripper()
        {
            if ( gripper_ctl.BUSY || !gripper_ctl.Ready || gripper_ctl.ALARM )
                return 1;

            gripper_ctl.DataFlag = 65535;
            gripper_ctl.MovementMode = 1;
            gripper_ctl.Positon = 4800;
            gripper_ctl.Speed = 100;
            gripper_ctl.Acc = 100;
            gripper_ctl.Dec = 100;
            gripper_ctl.Pusing_Force = 50;
            gripper_ctl.Trigger_LV = 40;
            gripper_ctl.Pushing_Speed = 10;
            gripper_ctl.Moving_Force = 100;
            gripper_ctl.Area_1 = 0;
            gripper_ctl.Area_2 = 0;
            gripper_ctl.In_Position = 5000;

            gripper_ctl.StartFlag = true;
            while (!gripper_ctl.BUSY && !gripper_ctl.ALARM)
                System.Threading.Thread.Sleep(gripper_check_interval);
            gripper_ctl.StartFlag = false;

            return gripper_ctl.ALARM ? 1:0;
        }

        #endregion

        /* Other Utils */
        #region Utils
        public int SysParamUpdate()
        {
            NexMotion_GroupAdapter groupAdapter = group_ctrl.GroupAdapter;
            RobotParameters groupParameters     = group_ctrl.GroupParameters;

            groupAdapter.NMC_GroupGetState(ref groupParameters.State);
            groupAdapter.NMC_GroupGetStatus(ref groupParameters.Status);
            groupAdapter.NMC_GroupGetActualPosAcs(ref groupParameters.PosAcs);
            groupAdapter.NMC_GroupGetActualPosPcs(ref groupParameters.PosPcs);

            // WTF...
            groupParameters.ActAcs = groupParameters.PosAcs.pos;
            groupParameters.ActPcs = groupParameters.PosPcs.pos;
            
            /* TODO: IO Sync? */

            return NexMotion_ErrCode.NMCERR_SUCCESS;
        }

        private bool CheckParam(string[] valid_status)
        {
            int valid_status_code = 0;
            int pos;
            foreach (string s in valid_status)
            {
                pos = Array.IndexOf(StatusMap, s);

                if (pos == -1) {
                    throw new System.ArgumentException(
                        String.Format("{0} is not an status in StatusMap", s),
                        "STATUS");
                } else {
                    valid_status_code |= 1 << pos;
                }
            }

            RobotParameters gp = group_ctrl.GroupParameters;
            if (gp.Status == valid_status_code) {
                return true;
            } else {
                return false;
            }
        }

        public void WaitStatus(int aim_status, int interval)
        {            
            int status = 0;
            int state = 0;
            do {
                group_ctrl.GroupAdapter.NMC_GroupGetState(ref state);
                group_ctrl.GroupAdapter.NMC_GroupGetStatus(ref status);
                System.Threading.Thread.Sleep(interval);
                
                if (state == NexMotion_Define.GROUP_STATE_ERROR)
                    throw new System.ArgumentException("State Error!", "STATE");

                if (state == NexMotion_Define.GROUP_STATE_STOPPED)
                    throw new System.ArgumentException("State Stoppedr!", "STATE");

            } while(status != aim_status);
        }

        public int WaitGripperBusy()
        {
            while (gripper_ctl.BUSY)
                System.Threading.Thread.Sleep(gripper_check_interval);

            return gripper_ctl.Alarm_1;
        }
        #endregion

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

        public string[] AvalibleStatus { get; set; } = new string[] {
            "ALL_STAND_STILL",
            "NO_POS_CHG",
        };

        private int[] HomingOpcodes = new int[] {
            20, 20, 22, 20, 22, 20,
            35 // Default is 35
        };

        private int gripper_check_interval = 100;
    }
}
