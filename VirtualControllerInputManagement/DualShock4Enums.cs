using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualControllerInputManagement
{
    public class DualShock4Enums
    {
        public enum DPadState : byte
        {
            Up = 0,
            UpRight = 1,
            Right = 2,
            DownRight = 3, 
            Down = 4,
            DownLeft = 5,
            Left = 6,
            UpLeft = 7,
            Released = 8,
        }

        public enum BatteryCharge : byte
        { 
            Bat0 = 0,
            Bat12 = 1,
            Bat25 = 2,
            Bat37 = 3,
            Bat50 = 4,
            Bat62 = 5,
            Bat75 = 6,
            Bat87 = 7,
            Bat100 = 8,
        }
    }
}
