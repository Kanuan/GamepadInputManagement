using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualControllerInputManagement
{
    public class DualShock4Input
    {
        private byte[] _inputBuffer = new byte[64];

        private int _offset = 0; 
        public int Offset { get => _offset; set => _offset = value; }

        public byte[] InputBuffer { get => _inputBuffer; set => _inputBuffer = value; }

        public byte ReportId { get => _inputBuffer[0 + Offset]; set => _inputBuffer[0 + Offset] = value; }
        public byte Axis_LsX { get => _inputBuffer[1 + Offset]; set => _inputBuffer[1 + Offset] = value; }
        public byte Axis_LsY { get => _inputBuffer[2 + Offset]; set => _inputBuffer[2 + Offset] = value; }
        public byte Axis_RsX { get => _inputBuffer[3 + Offset]; set => _inputBuffer[3 + Offset] = value; }
        public byte Axis_RsY { get => _inputBuffer[4 + Offset]; set => _inputBuffer[4 + Offset] = value; }
        public byte DPadState
        {
            get => (byte)(_inputBuffer[5 + Offset] & 0x0F);
            set => _inputBuffer[5 + Offset] = (byte)((_inputBuffer[5 + Offset] & 0xF0) | (value & 0x0F));
        }
        public bool Btn_Square
        {
            get => Utilities.getBitStateInByte(_inputBuffer[5 + Offset], 4);
            set => Utilities.setBitStateInByte(_inputBuffer[5 + Offset], 4, value);
        }

        public bool Btn_Cross
        {
            get => Utilities.getBitStateInByte(_inputBuffer[5 + Offset], 5);
            set => Utilities.setBitStateInByte(_inputBuffer[5 + Offset], 5, value);
        }

        public bool Btn_Circle
        {
            get => Utilities.getBitStateInByte(_inputBuffer[5 + Offset], 6);
            set => Utilities.setBitStateInByte(_inputBuffer[5 + Offset], 6, value);
        }

        public bool Btn_Triang
        {
            get => Utilities.getBitStateInByte(_inputBuffer[5 + Offset], 7);
            set => Utilities.setBitStateInByte(_inputBuffer[5 + Offset], 7, value);
        }

        public bool Btn_L1
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 0);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 0, value);
        }

        public bool Btn_R1
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 1);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 1, value);
        }

        public bool Btn_L2
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 2);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 2, value);
        }

        public bool Btn_R2
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 3);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 3, value);
        }

        public bool Btn_Share
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 4);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 4, value);
        }

        public bool Btn_Options
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 5);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 5, value);
        }

        public bool Btn_L3
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 6);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 6, value);
        }

        public bool Btn_R3
        {
            get => Utilities.getBitStateInByte(_inputBuffer[6 + Offset], 7);
            set => Utilities.setBitStateInByte(_inputBuffer[6 + Offset], 7, value);
        }

        public bool Btn_PS
        {
            get => Utilities.getBitStateInByte(_inputBuffer[7 + Offset], 0);
            set => Utilities.setBitStateInByte(_inputBuffer[7 + Offset], 0, value);
        }

        public bool Btn_TouchPad
        {
            get => Utilities.getBitStateInByte(_inputBuffer[7 + Offset], 1);
            set => Utilities.setBitStateInByte(_inputBuffer[7 + Offset], 1, value);
        }

        public byte Counter_InputRepTracker
        {
            get => (byte)(_inputBuffer[7 + Offset] >> 2);
            set
            {
                _inputBuffer[7 + Offset] = (byte)(value << 2);
            }
        }

        public byte Axis_L2
        {
            get => _inputBuffer[8 + Offset]; set => _inputBuffer[8 + Offset] = value;
        }

        public byte Axis_R2
        {
            get => _inputBuffer[9 + Offset]; set => _inputBuffer[9 + Offset] = value;
        }

        public ushort Counter_TimeStamp
        {
            get => BitConverter.ToUInt16(_inputBuffer, 10 + Offset);
            set
            {
                _inputBuffer[10 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[11 + Offset] = (byte)(value >> 4);
            }
        }

        public byte BatteryLevel
        {
            get => _inputBuffer[12 + Offset]; set => _inputBuffer[12 + Offset] = value;
        }

        public ushort Axis_GyroX
        {
            get => BitConverter.ToUInt16(_inputBuffer, 13 + Offset);
            set
            {
                _inputBuffer[13 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[14 + Offset] = (byte)(value >> 4);
            }
        }

        public ushort Axis_GyroY
        {
            get => BitConverter.ToUInt16(_inputBuffer, 15 + Offset);
            set
            {
                _inputBuffer[15 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[16 + Offset] = (byte)(value >> 4);
            }
        }

        public ushort Axis_GyroZ
        {
            get => BitConverter.ToUInt16(_inputBuffer, 17 + Offset);
            set
            {
                _inputBuffer[17 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[18 + Offset] = (byte)(value >> 4);
            }
        }

        public short Axis_AccelX
        {
            get => BitConverter.ToInt16(_inputBuffer, 19 + Offset);
            set
            {
                _inputBuffer[19 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[20 + Offset] = (byte)(value >> 4);
            }
        }

        public short Axis_AccelY
        {
            get => BitConverter.ToInt16(_inputBuffer, 21 + Offset);
            set
            {
                _inputBuffer[21 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[22 + Offset] = (byte)(value >> 4);
            }
        }

        public short Axis_AccelZ
        {
            get => BitConverter.ToInt16(_inputBuffer, 23 + Offset);
            set
            {
                _inputBuffer[23 + Offset] = (byte)(value & 0x0F);
                _inputBuffer[24 + Offset] = (byte)(value >> 4);
            }
        }

        public byte ExtData
        {
            get => _inputBuffer[30 + Offset]; set => _inputBuffer[30 + Offset] = value;
        }

        public byte TouchPadEventSeter
        {
            get => (byte)(_inputBuffer[33 + Offset] & 0x3);
            set => _inputBuffer[33 + Offset] = (byte)((_inputBuffer[33 + Offset] & 0xFC) | (value & 0x3));
        }

        public byte Counter_TouchPadGeneralActivityTracker
        {
            get => _inputBuffer[34 + Offset]; set => _inputBuffer[34 + Offset] = value;
        }

        public bool isCurrentTouchP0InContact
        {
            get => !(Utilities.getBitStateInByte(InputBuffer[35 + Offset], 7));
            set
            {
                InputBuffer[35 + Offset] = Utilities.setBitStateInByte(InputBuffer[35 + Offset], 7, !value);
            }

        }

        public byte Counter_CurrentTouchP0
        {
            get => (byte)(_inputBuffer[35 + Offset] & 0x7F);
            set => _inputBuffer[35 + Offset] = (byte)((_inputBuffer[35 + Offset] & 0x80) | (value & 0x7F));
        }

        public ushort Axis_CurrentTouchP0X
        {
            get => (ushort)(((_inputBuffer[37 + Offset] & 0x0f) << 8) | _inputBuffer[36 + Offset]);
            set
            {
                _inputBuffer[36 + Offset] = unchecked((byte)value);
                _inputBuffer[37 + Offset] = (byte)((_inputBuffer[37 + Offset] & 0xf0) | ((value >> 8) & 0x0f));
            }
        }

        public ushort Axis_CurrentTouchP0Y
        {
            get => (ushort)((_inputBuffer[38 + Offset] << 4) | ((_inputBuffer[37 + Offset] & 0xf0) >> 4));
            set
            {
                _inputBuffer[37 + Offset] = (byte)(((value << 4) & 0xf0) | (_inputBuffer[37 + Offset] & 0x0f));
                _inputBuffer[38 + Offset] = (byte)(value >> 4);
            }
        }

        public bool isCurrentTouchP1InContact
        {
            get => !Utilities.getBitStateInByte(InputBuffer[39 + Offset], 7);
            set => InputBuffer[39 + Offset] = Utilities.setBitStateInByte(InputBuffer[39 + Offset], 7, !value);
        }

        public byte Counter_CurrentTouchP1
        {
            get => (byte)(_inputBuffer[39 + Offset] & 0x7F);
            set => _inputBuffer[39 + Offset] = (byte)((_inputBuffer[39 + Offset] & 0x80) | (value & 0x7F));
        }

        public ushort Axis_CurrentTouchP1X
        {
            get => (ushort)(((_inputBuffer[41 + Offset] & 0x0f) << 8) | _inputBuffer[40 + Offset]);
            set
            {
                _inputBuffer[40 + Offset] = unchecked((byte)value);
                _inputBuffer[41 + Offset] = (byte)((_inputBuffer[41 + Offset] & 0xf0) | ((value >> 8) & 0x0f));
            }
        }

        public ushort Axis_CurrentTouchP1Y
        {
            get => (ushort)((_inputBuffer[42 + Offset] << 4) | ((_inputBuffer[41 + Offset] & 0xf0) >> 4));
            set
            {
                _inputBuffer[41 + Offset] = (byte)(((value << 4) & 0xf0) | (_inputBuffer[41 + Offset] & 0x0f));
                _inputBuffer[42 + Offset] = (byte)(value >> 4);
            }
        }

        public DualShock4Input(int offset)
        {
            _offset = offset;
        }

    }
}
