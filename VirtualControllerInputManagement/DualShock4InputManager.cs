using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualControllerInputManagement
{
    public class DualShock4InputManager
    {
        public DualShock4Input ds4;

              // ----------------------------- Start of Touchpad block --------------


        private bool[] _hasTouchXStateChanged = { false, false };
        public bool[] HasTouchXStateChanged
        {
            get => _hasTouchXStateChanged;
            set => _hasTouchXStateChanged = value;
        }

        private Stopwatch watch = new();

        private long watchTicksCurrent;
        private long watchTicksPrevious;


        public DualShock4InputManager()
        {
            ds4 = new();
            watchTicksCurrent = watchTicksPrevious = watch.ElapsedTicks;
        }


        // ----------------------------- End of Touchpad block --------------
        public void UpdateTouch(bool t0InContact, ushort t0PosX, ushort t0PosY, bool t1InContact, ushort t1PosX, ushort t1PosY)
        {
            // This checks for an impossible situation in which only a "touch 1" data arrives when no fingers are in the touchpad
            // If this happens, then touch 1 data is supposed to be touch 0
            // This also considers that touch 1 was previously present as is just being updated while touch 0 is absent
            if ((!t0InContact && t1InContact) && (!ds4.isCurrentTouchP0InContact && !ds4.isCurrentTouchP1InContact))
            {
                (t0InContact, t1InContact) = (t1InContact, t0InContact);
                (t0PosX, t1PosX) = (t1PosX, t0PosX);
                (t0PosY, t1PosY) = (t1PosY, t0PosY);
            }

            bool[] tXInContact = new bool[] { t0InContact, t1InContact };
            ushort[] tXPosX = new ushort[] { t0PosX, t1PosX };
            ushort[] tXPosY = new ushort[] { t0PosY, t1PosY };


            for (int i = 0; i < 2; i++)
            {
                if(tXInContact[i])
                {
                    if (!getTouchPXInContact(i) || tXPosX[i] != getTouchPXPosX(i) || tXPosY[i] != getTouchPXPosY(i)) // Check if anything changed
                    {
                        if (!getTouchPXInContact(i))  // update counter if finger was NOT touching the pad in previous inputRep
                            increaseTouchPXCounter(i);

                        setTouchPXPosX(i, tXPosX[i]); // Only update positions if finger is touching
                        setTouchPXPosY(i, tXPosY[i]);

                        HasTouchXStateChanged[i] = true;
                    }
                }
                else
                {
                    if (getTouchPXInContact(i)) // Check if finger is touching now but wasn't before
                    {
                        increaseTouchPXCounter(i);
                        HasTouchXStateChanged[i] = true;
                    }
                }
                setTouchPXInContact(i, tXInContact[i]);
            }


            byte tempTicks = unchecked((byte)(watch.ElapsedMicroSeconds() / 500));
            if (tempTicks > 127) tempTicks = (byte)127;

            if (HasTouchXStateChanged[0] || HasTouchXStateChanged[1])
            {
                ds4.Counter_TouchPadActivityTracker += tempTicks;
                Console.Write($"\nTouchpad activity counter delta from last inpRep:");
                Console.Write($"\n{tempTicks} (emulated)");
            }



            HasTouchXStateChanged[0] = false;
            HasTouchXStateChanged[1] = false;

            ds4.Counter_InputRepTracker++; ;
            watch.Restart();
        }

        private bool getTouchPXInContact(int i)
        {
            if (i == 0) return ds4.isCurrentTouchP0InContact;
            if (i == 1) return ds4.isCurrentTouchP1InContact;
            return false;
        }

        private void setTouchPXInContact(int i, bool set)
        {
            if (i == 0) ds4.isCurrentTouchP0InContact = set;
            if (i == 1) ds4.isCurrentTouchP1InContact = set; 
        }

        private byte getTouchPXCounter(int i)
        {
            if (i == 0) return ds4.Counter_CurrentTouchP0;
            if (i == 1) return ds4.Counter_CurrentTouchP1;
            return 0;
        }

        private void increaseTouchPXCounter(int i)
        {
            if (i == 0) ds4.Counter_CurrentTouchP0++;
            if (i == 1) ds4.Counter_CurrentTouchP1++;
        }

        private ushort getTouchPXPosX(int i)
        {
            if (i == 0) return ds4.Axis_CurrentTouchP0X;
            if (i == 1) return ds4.Axis_CurrentTouchP1X;
            return 0;
        }

        private void setTouchPXPosX(int i, ushort pos)
        {
            if (i == 0) ds4.Axis_CurrentTouchP0X = pos;
            if (i == 1) ds4.Axis_CurrentTouchP1X = pos;
        }

        private ushort getTouchPXPosY(int i)
        {
            if (i == 0) return ds4.Axis_CurrentTouchP0Y;
            if (i == 1) return ds4.Axis_CurrentTouchP1Y;
            return 0;
        }

        private void setTouchPXPosY(int i, ushort pos)
        {
            if (i == 0) ds4.Axis_CurrentTouchP0Y = pos;
            if (i == 1) ds4.Axis_CurrentTouchP1Y = pos;
        }
    }
}
