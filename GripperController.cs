using System;
using NEXCOMROBOT.MCAT;

namespace NEXCOMROBOT
{
    public class GripperController
    {
        enum InputPort_Mask : ushort
        {
            OUT0 = 1 << 0,
            OUT1 = 1 << 1,
            OUT2 = 1 << 2,
            OUT3 = 1 << 3,
            OUT4 = 1 << 4,
            OUT5 = 1 << 5,
            BUSY = 1 << 8,
            SVRE = 1 << 9,
            SETON = 1 << 10,
            INP = 1 << 11,
            AREA = 1 << 12,
            WAREA = 1 << 13,
            ESTOP = 1 << 14,
            ALARM = 1 << 15
        }

        enum OutputPort_Mask : ushort
        {
            IN0 = 1 << 0,
            IN1 = 1 << 1,
            IN2 = 1 << 2,
            IN3 = 1 << 3,
            IN4 = 1 << 4,
            IN5 = 1 << 5,
            HOLD = 1 << 8,
            SVON = 1 << 9,
            DRIVE = 1 << 10,
            RESET = 1 << 11,
            SETUP = 1 << 12,
            JOG_N = 1 << 13,
            JOG_P = 1 << 14,
            FLGTH = 1 << 15
        }

        private readonly NexMotion_IOAdapter mIoAdapter = null;


        private NexMotionDataValue mtempValue = new NexMotionDataValue();

        #region TxPDO OFFSET
        private uint INPUT_DATA_OFFSET;
        private uint CONTROLLER_FLAG_OFFSET;
        private uint CURRENT_POSITION_OFFSET;
        private uint CURRENT_SPEED_OFFSET;
        private uint CURRENT_PUSHGIN_FORCE_OFFSET;
        private uint SET_TARGET_POSITION_OFFSET;
        private uint ALARM1_OFFSET;
        #endregion

        #region RxPDO OFFSET
        private uint OUTPUT_DATA_OFFSET;
        private uint DATA_FLAG_OFFSET;
        private uint START_FLAG_OFFSET;
        private uint MOVEMENT_MODE_OFFSET;
        private uint SPEED_OFFSET;
        private uint TARGET_POSITION_OFFSET;
        private uint ACC_OFFSET;
        private uint DEC_OFFSET;
        private uint PUSHING_FORCE_OFFSET;
        private uint TRIGGER_LV_OFFSET;
        private uint PUSHING_SPEED_OFFSET;
        private uint MOVEING_FORCE_OFFSET;
        private uint AREA1_OFFSET;
        private uint AREA2_OFFSET;
        private uint IN_POSINTION_OFFSET;
        #endregion

        public GripperController(NexMotion_IOAdapter ioadapter, uint tx_offset, uint rx_offset)
        {
            /* TxPDO OFFSET */
            INPUT_DATA_OFFSET = tx_offset;
            CONTROLLER_FLAG_OFFSET = INPUT_DATA_OFFSET + 2;
            CURRENT_POSITION_OFFSET = INPUT_DATA_OFFSET + 4;
            CURRENT_SPEED_OFFSET = INPUT_DATA_OFFSET + 8;
            CURRENT_PUSHGIN_FORCE_OFFSET = INPUT_DATA_OFFSET + 10;
            SET_TARGET_POSITION_OFFSET = INPUT_DATA_OFFSET + 12;
            ALARM1_OFFSET = INPUT_DATA_OFFSET + 16;

            /* RxPDO OFFSET */
            OUTPUT_DATA_OFFSET = rx_offset;
            DATA_FLAG_OFFSET = OUTPUT_DATA_OFFSET + 2;
            START_FLAG_OFFSET = OUTPUT_DATA_OFFSET + 4;
            MOVEMENT_MODE_OFFSET = OUTPUT_DATA_OFFSET + 5;
            SPEED_OFFSET = OUTPUT_DATA_OFFSET + 6;
            TARGET_POSITION_OFFSET = OUTPUT_DATA_OFFSET + 8;
            ACC_OFFSET = OUTPUT_DATA_OFFSET + 12;
            DEC_OFFSET = OUTPUT_DATA_OFFSET + 14;
            PUSHING_FORCE_OFFSET = OUTPUT_DATA_OFFSET + 16;
            TRIGGER_LV_OFFSET = OUTPUT_DATA_OFFSET + 18;
            PUSHING_SPEED_OFFSET = OUTPUT_DATA_OFFSET + 20;
            MOVEING_FORCE_OFFSET = OUTPUT_DATA_OFFSET + 22;
            AREA1_OFFSET = OUTPUT_DATA_OFFSET + 24;
            AREA2_OFFSET = OUTPUT_DATA_OFFSET + 28;
            IN_POSINTION_OFFSET = OUTPUT_DATA_OFFSET + 32;

            mIoAdapter = ioadapter;
        }

        #region TxPDO

        #region 6010h Input Data

        private bool GetInputBitValue(ushort mask)
        {
            return ((InputPort & mask) > 0) ? true : false;
        }

        public ushort InputPort
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(INPUT_DATA_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
        }

        public bool BUSY
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.BUSY); }
        }

        public bool SVRE
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.SVRE); }
        }

        public bool SETON
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.SETON); }
        }

        public bool INP
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.INP); }
        }

        public bool AREA
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.AREA); }
        }
        public bool WAREA
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.WAREA); }
        }

        public bool ESTOP
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.ESTOP); }
        }

        public bool ALARM
        {
            get { return GetInputBitValue((ushort)InputPort_Mask.ALARM); }
        }

        #endregion

        #region 6011h Ready
        public bool Ready
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(CONTROLLER_FLAG_OFFSET, 2, ref mtempValue);
                return ( (mtempValue.u16 & (1 << 4) ) > 0) ? true : false;
            }
        }
        #endregion

        #region 6020h Current Position
        public UInt32 Current_Position
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(CURRENT_POSITION_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
        }
        #endregion

        #region 6021h Current Speed
        public ushort Current_Speed
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(CURRENT_SPEED_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
        }
        #endregion

        #region 6022h Current Pushing Force
        public ushort Current_Pushing_Force
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(CURRENT_PUSHGIN_FORCE_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
        }
        #endregion

        #region 6023h Set Target Position
        public UInt32 Set_Target_Position
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(SET_TARGET_POSITION_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
        }
        #endregion

        #region 6030h Alarm 1
        public byte Alarme_1
        {
            get
            {
                mIoAdapter.NMC_ReadInputMemory(ALARM1_OFFSET, 1, ref mtempValue);
                return mtempValue.u8;
            }
        }
        #endregion

        #endregion

        #region RxPDO

        #region 7010h Ouput Data
        private ushort OutputPort
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(OUTPUT_DATA_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(OUTPUT_DATA_OFFSET, 2, ref mtempValue);
            }
        }

        public bool IN1
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.IN1); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.IN1, value); }
        }

        public bool SVON
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.SVON); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.SVON, value); }
        }

        public bool DRIVE
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.DRIVE); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.DRIVE, value); }
        }

        public bool RESET
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.RESET); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.RESET, value); }
        }

        public bool SETUP
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.SETUP); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.SETUP, value); }
        }

        public bool JOG_N
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.JOG_N); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.JOG_N, value); }
        }

        public bool JOG_P
        {
            get { return GetOutputBitValue((ushort)OutputPort_Mask.JOG_P); }
            set { SetOutputBitValue((ushort)OutputPort_Mask.JOG_P, value); }
        }

        private bool GetOutputBitValue(ushort mask)
        {
            return ((OutputPort & mask) > 0) ? true : false;
        }

        private void SetOutputBitValue(ushort mask, bool value)
        {
            if (value)
                OutputPort = (ushort)(OutputPort | mask);
            else
                OutputPort = (ushort)(OutputPort & ~mask);
        }
        #endregion

        #region 7011h Data Flag
        public ushort DataFlag
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(DATA_FLAG_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(DATA_FLAG_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7012h Start Flag
        public bool StartFlag
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(START_FLAG_OFFSET, 1, ref mtempValue);
                return ((mtempValue.u8 & (1 << 0)) > 0) ? true : false;
            }
            set
            {
                if (value)
                    mtempValue.u8 = 1;
                else
                    mtempValue.u8 = 0;

                mIoAdapter.NMC_WriteOutputMemory(START_FLAG_OFFSET, 1, ref mtempValue);
            }
        }
        #endregion

        #region 7020h Movement Mode
        public byte MovementMode
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(MOVEMENT_MODE_OFFSET, 1, ref mtempValue);
                return mtempValue.u8;
            }
            set
            {
                mtempValue.u8 = value;
                mIoAdapter.NMC_WriteOutputMemory(MOVEMENT_MODE_OFFSET, 1, ref mtempValue);
            }
        }
        #endregion

        #region 7021h Speed
        public ushort Speed
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(SPEED_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(SPEED_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7022h Positon
        public UInt32 Positon
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(TARGET_POSITION_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
            set
            {
                mtempValue.u32 = value;
                mIoAdapter.NMC_WriteOutputMemory(TARGET_POSITION_OFFSET, 4, ref mtempValue);
            }
        }
        #endregion

        #region 7023h Acceleration
        public ushort Acc
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(ACC_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(ACC_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7024h Deceleration
        public ushort Dec
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(DEC_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(DEC_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7025h Pushing Force
        public ushort Pusing_Force
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(PUSHING_FORCE_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(PUSHING_FORCE_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7026h Trigger LV
        public ushort Trigger_LV
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(TRIGGER_LV_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(TRIGGER_LV_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7027h Pushing Speed
        public ushort Pushing_Speed
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(PUSHING_SPEED_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(PUSHING_SPEED_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7028h Moving Force
        public ushort Moving_Force
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(MOVEING_FORCE_OFFSET, 2, ref mtempValue);
                return mtempValue.u16;
            }
            set
            {
                mtempValue.u16 = value;
                mIoAdapter.NMC_WriteOutputMemory(MOVEING_FORCE_OFFSET, 2, ref mtempValue);
            }
        }
        #endregion

        #region 7029h Area 1
        public UInt32 Area_1
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(AREA1_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
            set
            {
                mtempValue.u32 = value;
                mIoAdapter.NMC_WriteOutputMemory(AREA1_OFFSET, 4, ref mtempValue);
            }
        }
        #endregion

        #region 702Ah Area 2
        public UInt32 Area_2
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(AREA2_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
            set
            {
                mtempValue.u32 = value;
                mIoAdapter.NMC_WriteOutputMemory(AREA2_OFFSET, 4, ref mtempValue);
            }
        }
        #endregion

        #region 702Bh In Position
        public UInt32 In_Position
        {
            get
            {
                mIoAdapter.NMC_ReadOutputMemory(IN_POSINTION_OFFSET, 4, ref mtempValue);
                return mtempValue.u32;
            }
            set
            {
                mtempValue.u32 = value;
                mIoAdapter.NMC_WriteOutputMemory(IN_POSINTION_OFFSET, 4, ref mtempValue);
            }
        }
        #endregion

        #endregion
    }
}
