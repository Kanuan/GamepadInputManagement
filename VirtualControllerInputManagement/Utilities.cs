using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualControllerInputManagement
{
    public static class Utilities
    {
        public static bool getBitStateInByte(byte inByte, int bitPos)
        {
            return Convert.ToBoolean(inByte & (1U << bitPos)) ? true : false;
        }

        public static byte setBitStateInByte(byte inByte, int bitPos, bool setBit)
        {
            if (setBit) inByte |= (byte)(1U << bitPos);
            else inByte &= (byte)~(1U << bitPos);
            return inByte;
        }

        public static long ElapsedNanoSeconds(this Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }
        public static long ElapsedMicroSeconds(this Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
        }
    }
}
