using System;
using NEXCOMROBOT.MCAT;

namespace NEXCOMROBOT
{
    public class RobotAgent
    {
        public GroupControl group_ctrl;
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
        public int Enable()
        {
            int ret = group_ctrl.GroupAdapter.NMC_GroupEnable();
            EnableGripper();

            return ret;
        }

        public int Disable()
        {
            int ret = group_ctrl.GroupAdapter.NMC_GroupDisable();
            DisableGripper();

            return ret;
        }

        public int Reset()
        {
            int ret;
            ret = group_ctrl.GroupAdapter.NMC_GroupDisable();
            if ( ret != NexMotion_ErrCode.NMCERR_SUCCESS ) return ret;

            ret = group_ctrl.GroupAdapter.NMC_GroupResetState();
            if ( ret != NexMotion_ErrCode.NMCERR_SUCCESS ) return ret;

            DisableGripper();
            ResetGripper();

            return ret;
        }
        #endregion
        
        /* State Getting */
        #region StateGetting
        public RobotStatus GetStatus()
        {
            SysParamUpdate();

            var group_parameters = group_ctrl.GroupParameters;
            RobotStatus robot_status = new RobotStatus();
            robot_status.state = group_parameters.State;
            robot_status.status = group_parameters.Status;
            robot_status.acs = group_parameters.ActAcs;
            robot_status.pcs = group_parameters.ActPcs;
            robot_status.gripper_status = GetGripperStatus();

            return robot_status;
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
        public int SetHomePos(Pos_T home_pos)
        {
            int mask = 1 << group_ctrl.AxisCount - 1;

            int ret = group_ctrl.GroupAdapter.NMC_GroupSetHomePos(mask, ref home_pos);
            SysParamUpdate();

            return ret;
        }

        public int SetVelocity(double[] vel)
        {
            int param_num = 0x32;
            int sub_index = 0;
            int ret = 0;

            for (int axis_index = 0; axis_index < group_ctrl.AxisCount; ++ axis_index)
            {
                ret = group_ctrl.GroupAdapter.NMC_GroupAxSetParamF64(
                    axis_index, param_num, sub_index,
                    vel[axis_index]
                );

                if ( ret != NexMotion_ErrCode.NMCERR_SUCCESS ) break;
            }
            SysParamUpdate();

            return ret;
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

            if ( ret != NexMotion_ErrCode.NMCERR_SUCCESS ) return ret;

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
            if ( ret != NexMotion_ErrCode.NMCERR_SUCCESS ) return ret;

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
        public int EnableGripper()
        {
            int duration;
            gripper_ctl.SVON = true;

            DateTime start_time = DateTime.Now;
            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
                duration = (int)((TimeSpan)(DateTime.Now - start_time)).TotalMilliseconds;
            } while (!gripper_ctl.SVRE && duration < max_timeout);

            return duration < max_timeout ? 0 : 1;
        }

        public int DisableGripper()
        {
            int duration;
            gripper_ctl.SVON = false;

            DateTime start_time = DateTime.Now;
            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
                duration = (int)((TimeSpan)(DateTime.Now - start_time)).TotalMilliseconds;
            } while (gripper_ctl.SVRE && duration < max_timeout);

            return duration < max_timeout ? 0 : 1;
        }

        public int ResetGripper()
        {
            int duration;
            gripper_ctl.RESET = true;

            DateTime start_time = DateTime.Now;
            do {
                System.Threading.Thread.Sleep(gripper_check_interval);
                duration = (int)((TimeSpan)(DateTime.Now - start_time)).TotalMilliseconds;
            } while (gripper_ctl.ALARM && duration < max_timeout);

            gripper_ctl.RESET = false;

            return duration < max_timeout ? 0 : 1;
        }
        #endregion

        /* Gripper State Getting */
        #region GripperStateGetting
        public GripperStatus GetGripperStatus()
        {
            if (gripper_ctl == default(GripperController))
                return null;

            GripperStatus gripper_status = new GripperStatus();
            gripper_status.is_gripped = gripper_ctl.INP;
            gripper_status.current_pos = gripper_ctl.Current_Position / 100.0;
            gripper_status.alarm_code = gripper_ctl.Alarm_1;
            gripper_status.is_ready = gripper_ctl.Ready;
            gripper_status.is_busy = gripper_ctl.BUSY;

            return gripper_status;
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
            gripper_ctl.Positon = 4700;
            gripper_ctl.Speed = 100;
            gripper_ctl.Acc = 100;
            gripper_ctl.Dec = 100;
            gripper_ctl.Pusing_Force = 50;
            gripper_ctl.Trigger_LV = 40;
            gripper_ctl.Pushing_Speed = 10;
            gripper_ctl.Moving_Force = 100;
            gripper_ctl.Area_1 = 0;
            gripper_ctl.Area_2 = 0;
            gripper_ctl.In_Position = 0;

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
                pos = Array.IndexOf(StateMaps.RobotStatusMap, s);

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

        public int WaitStatus(int aim_status, int interval, int timeout = int.MaxValue)
        {            
            int status = 0;
            int state = 0;
            int duration;

            DateTime start_time = DateTime.Now;
            do {
                group_ctrl.GroupAdapter.NMC_GroupGetState(ref state);
                group_ctrl.GroupAdapter.NMC_GroupGetStatus(ref status);
                System.Threading.Thread.Sleep(interval);
                
                if (state == NexMotion_Define.GROUP_STATE_ERROR) {
                    this.EMGStop();
                    throw new System.ArgumentException("State Error!", "STATE");
                }

                if (state == NexMotion_Define.GROUP_STATE_STOPPED) {
                    this.EMGStop();
                    throw new System.ArgumentException("State Stopped!", "STATE");
                }

                duration = (int)((TimeSpan)(DateTime.Now - start_time)).TotalMilliseconds;
            } while( status != aim_status && duration < timeout );

            return duration < timeout ? 0 : 1;
        }

        public int WaitGripperBusy(int interval = 100, int timeout = int.MaxValue)
        {
            DateTime start_time = DateTime.Now;
            while ( gripper_ctl.BUSY &&
                ((TimeSpan)(DateTime.Now - start_time)).TotalMilliseconds < timeout )
                System.Threading.Thread.Sleep(interval);

            return gripper_ctl.Alarm_1;
        }
        #endregion

        /* Decoder */
        public string[] AvalibleStatus { get; set; } = new string[] {
            "ALL_STAND_STILL",
            "NO_POS_CHG",
        };

        private int[] HomingOpcodes = new int[] {
            20, 20, 22, 20, 22, 20,
            35 // Default is 35
        };

        private int gripper_check_interval = 100;
        private int max_timeout = 10000;
    }
}
