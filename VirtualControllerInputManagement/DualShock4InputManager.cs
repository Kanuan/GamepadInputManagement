using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VirtualControllerInputManagement
{
    public class DualShock4InputManager
    {
        public DualShock4Input DS4 = new();

        // ----------------------------- Start of Touchpad block --------------


        private bool[] _hasTouchXStateChanged = { false, false };
        public bool[] HasTouchXDataChanged
        {
            get => _hasTouchXStateChanged;
            set => _hasTouchXStateChanged = value;
        }

        private Stopwatch watch = new();

        private int touchPad_TicksSinceLastReport { get; set; }

        private bool isSetInvertTouchData = false;
        private bool[] hasTouchDataChange = { false, false };

        private byte Counter_TouchPadFingerDown { get; set; }



        public DualShock4InputManager(DualShock4Input inDs4)
        {
            DS4 = inDs4;
            DS4 = new();
            Counter_TouchPadFingerDown = (DS4.Counter_CurrentTouchP0 >= DS4.Counter_CurrentTouchP1) ? DS4.Counter_CurrentTouchP0 : DS4.Counter_CurrentTouchP1;
        }


        public void UpdateTouch(bool t0InContact, ushort t0PosX, ushort t0PosY, bool t1InContact, ushort t1PosX, ushort t1PosY)
        {
            if (isSetInvertTouchData && !t0InContact && !t1InContact)
            {
                isSetInvertTouchData = false;
            }

            // Swap touch data if ONLY TouchP1 is in contact currently but neither TouchP0 or TouchP1 was in contact in the previous report
            // Reason: impossible for no fingers to be in contact then the 1st fingerto get in contact to be considered "2nd finger"
            isSetInvertTouchData = (!t0InContact & t1InContact) && (!DS4.isCurrentTouchP0InContact & !DS4.isCurrentTouchP1InContact);
            if (isSetInvertTouchData)
            {
                (t0InContact, t1InContact) = (t1InContact, t0InContact);
                (t0PosX, t1PosX) = (t1PosX, t0PosX);
                (t0PosY, t1PosY) = (t1PosY, t0PosY);
            }

            // Organize data in arrays
            bool[] tXInContact = new bool[] { t0InContact, t1InContact };
            ushort[] tXPosX = new ushort[] { t0PosX, t1PosX };
            ushort[] tXPosY = new ushort[] { t0PosY, t1PosY };

            // Process each TouchPX data individually
            // PosX/Y must only be updated if finger is in contact
            // Counter_TouchPadFingerDown must be incremented only once when touching the TP, but both fingers incremement the same counter
            // If the finger is in contact and there was ANY change in data regarding the previous input report, then set the "DataChanged" flag
            for (int i = 0; i < 2; i++)
            {
                if (tXInContact[i]) // Check if finger is touching
                {
                    if (HasTouchXDataChanged[i] = checkIfTouchPXDataChanged(i, tXInContact[i], tXPosX[i], tXPosY[i]))
                    {
                        setTouchPXPosX(i, tXPosX[i]);
                        setTouchPXPosY(i, tXPosY[i]);
                        if (!getTouchPXInContact(i)) // If it's the starting finger contact, increase counter used for both fingers but only sets current finger counter
                        {
                            Counter_TouchPadFingerDown++;
                            setTouchPXCounter(i, Counter_TouchPadFingerDown);
                        }
                    }
                }
                else  // Also set the "DataChanged" flag when finger is no longer in touch
                {
                    if (getTouchPXInContact(i)) // Check if finger was in contact before
                    {
                        HasTouchXDataChanged[i] = true;
                    }
                }
                setTouchPXInContact(i, tXInContact[i]);
            }

            // If there was touchpad activity, increase the general activity counter by the delta between current and previous "touchpad clock ticks"
            if (HasTouchXDataChanged[0] || HasTouchXDataChanged[1])
            {
                DS4.Counter_TouchPadGeneralActivityTracker += getTouchPadTicksDelta();
            }

            // Prepare for next report
            HasTouchXDataChanged[0] = false;
            HasTouchXDataChanged[1] = false;
            watch.Restart();

            DS4.Counter_InputRepTracker++; // Not part of the touchpad stuff. Move it from here later
        }

        /// <summary>
        /// Compares touch data received with the one already set to check if there is a difference
        /// </summary>
        /// <param name="i">Touch Data group index (0 = TouchP0, 1 = TouchP1)</param>
        /// <param name="tInContact">True if finger is in contact, false if not</param>
        /// <param name="tXPosX">Touch position in the Axis X</param>
        /// <param name="tXPosY">Touch position in the Axis Y</param>
        /// <returns>True if any of the values are different, false if they are equal</returns>
        private bool checkIfTouchPXDataChanged(int i, bool tInContact, ushort tXPosX, ushort tXPosY)
        {
            if (tInContact != getTouchPXInContact(i) || tXPosX != getTouchPXPosX(i) || tXPosY != getTouchPXPosY(i))
                return true;
            return false;
        }

        private byte getTouchPadTicksDelta()
        { 
            int ticksDelta = unchecked((int)(watch.ElapsedMicroSeconds() / 500)); // internal TP Counter seems to tick every 500 microseconds of touchpad activity
            if (ticksDelta > 127) ticksDelta = (byte)127;
            return (byte)ticksDelta;
        }

        private bool getTouchPXInContact(int i)
        {
            if (i == 0) return DS4.isCurrentTouchP0InContact;
            if (i == 1) return DS4.isCurrentTouchP1InContact;
            throw new IndexOutOfRangeException();
        }

        private void setTouchPXInContact(int i, bool set)
        {
            if (i == 0) DS4.isCurrentTouchP0InContact = set;
            if (i == 1) DS4.isCurrentTouchP1InContact = set;
            throw new IndexOutOfRangeException();
        }

        private byte getTouchPXCounter(int i)
        {
            if (i == 0) return DS4.Counter_CurrentTouchP0;
            if (i == 1) return DS4.Counter_CurrentTouchP1;
            throw new IndexOutOfRangeException();
        }

        private void setTouchPXCounter(int i, int value)
        {
            if (i == 0) DS4.Counter_CurrentTouchP0 = (byte)value;
            if (i == 1) DS4.Counter_CurrentTouchP1 = (byte)value;
            throw new IndexOutOfRangeException();
        }

        private ushort getTouchPXPosX(int i)
        {
            if (i == 0) return DS4.Axis_CurrentTouchP0X;
            if (i == 1) return DS4.Axis_CurrentTouchP1X;
            throw new IndexOutOfRangeException();
        }

        private void setTouchPXPosX(int i, ushort pos)
        {
            if (i == 0) DS4.Axis_CurrentTouchP0X = pos;
            if (i == 1) DS4.Axis_CurrentTouchP1X = pos;
            throw new IndexOutOfRangeException();
        }

        private ushort getTouchPXPosY(int i)
        {
            if (i == 0) return DS4.Axis_CurrentTouchP0Y;
            if (i == 1) return DS4.Axis_CurrentTouchP1Y;
            throw new IndexOutOfRangeException();
        }

        private void setTouchPXPosY(int i, ushort pos)
        {
            if (i == 0) DS4.Axis_CurrentTouchP0Y = pos;
            if (i == 1) DS4.Axis_CurrentTouchP1Y = pos;
            throw new IndexOutOfRangeException();
        }
    }
}
