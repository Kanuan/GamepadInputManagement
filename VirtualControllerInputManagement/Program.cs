using Nefarius.Drivers.WinUSB;
using Nefarius.Drivers.Identinator;
using Nefarius.Utilities.DeviceManagement.PnP;
using System.Diagnostics;

namespace VirtualControllerInputManagement
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WindowWidth = 200;
            var instance = 0;

            while (Devcon.FindByInterfaceGuid(FilterDriver.FilteredDeviceInterfaceId, out var path, out var instanceId,
                       instance++))
            {
                var device = PnPDevice.GetDeviceByInstanceId(instanceId);

                if (!Equals(device.GetProperty<string>(DevicePropertyDevice.Service), "WINUSB")) continue;

                // if (!Equals(device.GetProperty<string>(DevicePropertyDevice.FriendlyName), "eSwap PRO Controller")) continue;
                if (!Equals(device.GetProperty<string>(DevicePropertyDevice.FriendlyName), "Wireless Controller")) continue;

                var usbDevice = USBDevice.GetSingleDeviceByPath(path);

                var desc = usbDevice.Descriptor;

                Console.WriteLine($"Found {desc.FullName}, attempting to read...\r\n");

                var pipe = usbDevice.Pipes.First();

                var buffer = new byte[pipe.MaximumPacketSize];
                int read;

                DualShock4InputManager vds4 = new();

                bool[,] tXdown = new bool[2,2];
                byte[,] tXPosX = new byte[2,2];
                byte[,] tXPosY = new byte[2,2];

                int[] dif = new int[] { 0, 0, 0 };

                bool notDone = true;

                int autoCount = 0;

                ushort[,] finger0pos = new ushort[2,2];
                ushort[,] finger1pos = new ushort[2,2];

                long[] systemTimeInNS = new long[2] { Stopwatch.GetTimestamp(), Stopwatch.GetTimestamp() };
                double timeDelta = 0;
                double[] RealGenCounter = new double[2] { 0, 1 };

                double averageTickTime = 0;
                double counterDelta = 1;
                double timeBetweenCounterTicks = 1;

                DualShock4InputManager currentDS4Inp = new();
                DualShock4InputManager previousDS4Inp = new();
                DualShock4Input ds4Buffer = new();
                ds4Buffer.InputBuffer = buffer;

                Stopwatch stopwatch = new();

                Thread.Sleep(100);

                while ((read = pipe.Read(buffer)) > 0)
                {
                    if (notDone)
                    {
                        Array.Copy(buffer, currentDS4Inp.ds4.InputBuffer, 64);
                        notDone = false;
                        stopwatch.Start();
                    }
                    //Console.Write($"\rT1 down: {currentDS4Inp.isCurrentTouchP1InContact} // X: {currentDS4Inp.Axis_CurrentTouchP1X} Y:{currentDS4Inp.Axis_CurrentTouchP1Y} //  Cross: {currentDS4Inp.Btn_Cross} // R2: {currentDS4Inp.Axis_R2}                 ");

                    


                    currentDS4Inp.UpdateTouch(
                        ds4Buffer.isCurrentTouchP0InContact, ds4Buffer.Axis_CurrentTouchP0X, ds4Buffer.Axis_CurrentTouchP0Y,
                        ds4Buffer.isCurrentTouchP1InContact, ds4Buffer.Axis_CurrentTouchP1X, ds4Buffer.Axis_CurrentTouchP1Y
                        );

                    // Console.Write($"\r\nT0: {currentDS4Inp.ds4.isCurrentTouchP0InContact} // RealT: {ds4Buffer.isCurrentTouchP0InContact} // X: {currentDS4Inp.ds4.Axis_CurrentTouchP0X} Y:{currentDS4Inp.ds4.Axis_CurrentTouchP0Y}");

                    /*
                    if (!currentDS4Inp.ds4.isCurrentTouchP0InContact && previousDS4Inp.ds4.isCurrentTouchP0InContact)
                    {
                        Console.Write($"\r\nDetected finger 1 was lifted");
                        if (currentDS4Inp.ds4.Counter_TouchPadActivityTracker != previousDS4Inp.ds4.Counter_TouchPadActivityTracker)
                        {
                            Console.Write($"\r\nIn the InpRep that has the finger lifted action, it TouchPad General Counter was increased.");
                            Console.Write($"\r\nGeneral TP Counter dif was {currentDS4Inp.ds4.Counter_TouchPadActivityTracker - previousDS4Inp.ds4.Counter_TouchPadActivityTracker}.");
                            //Console.Write($"\r\nTime difference between events: {Utility.ElapsedMicroSeconds(stopwatch)} // pet tick ({Utility.ElapsedMicroSeconds(stopwatch)/ (RealGenCounter[0] - RealGenCounter[1])}).");

                        }
                    }
                    */

                    byte difTActivity = (byte)( ds4Buffer.Counter_TouchPadActivityTracker - previousDS4Inp.ds4.Counter_TouchPadActivityTracker );

                    if(difTActivity > 0)
                    if ( ( (int)(stopwatch.ElapsedMicroSeconds() / difTActivity)) > 0)
                    {
                        //Console.Write($"\r\nTime difference between events: {(int)stopwatch.ElapsedMicroSeconds()} // pet tick ({(int)(stopwatch.ElapsedMicroSeconds() / difTActivity)}).");
                        Console.Write($"\n{difTActivity} (real)");
                            Console.WriteLine();
                            averageTickTime = (averageTickTime + (int)(stopwatch.ElapsedMicroSeconds() / difTActivity)) / 2;
                        autoCount++;
                    }
                            stopwatch.Restart();





                    //Console.Write($"\r{new string(' ', Console.BufferWidth - 1)}");
                    //Console.Write($"\r{string.Join(" ", buffer.Take(read).Select(b => b.ToString("X2")))}");
                    //Console.Write($"\r{Convert.ToString(tempX, 2)}");






                    //if(autoCount == 0 || autoCount == 126)
                    // Console.Write($"\r\n{buffer[34]} {vds4.GeneralTouchPadCounterByte}     {buffer[7] >> 2} {vds4.GeralInpRepCounter}  {dif[0]}  {dif[1]}  {dif[2]}");
                    //Console.Write($"\r\nTime btw ticks: {timeBetweenCounterTicks} // Ticks dif: {counterDelta}");
                    // Thread.Sleep(2);



                    // Save previous data:

                    systemTimeInNS[1] = systemTimeInNS[0];
                    RealGenCounter[1] = RealGenCounter[0];

                    Array.Copy(buffer, previousDS4Inp.ds4.InputBuffer, 64);

                    

                    if (autoCount > 1000) break;

                }
                Console.Write($"\r\nAverage tick time: {averageTickTime}");
                break;
            }



        }

        private static void TestTimeDeltaOfRealTouchPadCounter()
        {
            /*
            if (notDone)
            {
                vds4.GeneralTouchPadCounterByte = buffer[34];
                notDone = false;
                previousByteCounter = buffer[34];

            }

            //if(curr)



            currentTime = Stopwatch.GetTimestamp();



            if (previousByteCounter != buffer[34])
            {
                currentRealGenCounter = currentRealGenCounter + (buffer[34] - previousByteCounter);
                counterDelta = currentRealGenCounter - previousRealGenCounter;
                timeDelta = currentTime - previousTime;
                timeBetweenCounterTicks = timeDelta / counterDelta;



                averageTickTime = (averageTickTime + timeBetweenCounterTicks) / 2;


                Console.Write($"\r\nTime btw ticks: {(int)timeBetweenCounterTicks} // Ticks dif: {counterDelta} // New touch counter: {buffer[34]} // Previous touch: {previousByteCounter}");
                autoCount++;
            }

            previousTime = currentTime;
            previousRealGenCounter = currentRealGenCounter;
            previousByteCounter = buffer[34];



            if (autoCount > 1000) break;
            */
        }

    }

}