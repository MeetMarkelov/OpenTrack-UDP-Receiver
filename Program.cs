using System;
using System.Threading;

namespace OpenTrackUDPReceiver
{
    class Program
    {
        public static void OnReceivedData(OpenTrackData Data)
        {
            Console.Write("X: " + Data.X + ", Y: " + Data.Y + ", z: " + Data.Z + "\n");
            Console.Write("yaw: " + Data.Yaw + ", pitch: " + Data.Pitch + ", roll: " + Data.Roll + "\n");
        }

        static void Main(string[] args)
        {
            int LocalPort = 4242;
            int ReceiveTimeout = 1000; // milliseconds

            while (true)
            {
                
                // First use case start
                /*
                OpenTrackData Data_First = new OpenTrackData();

                if (Data_First.ReceiveOpenTrackData(LocalPort, ReceiveTimeout))
                {
                    // Good data
                    Console.Write("Data_First: X: " + Data_First.X + ", Y: " + Data_First.Y + ", z: " + Data_First.Z + "\n");
                    Console.Write("yaw: " + Data_First.Yaw + ", pitch: " + Data_First.Pitch + ", roll: " + Data_First.Roll + "\n");
                }
                else
                {
                    // Empty data
                    Console.Write("No Data_First\n");
                }

                // First use case end
                // OR
                // Second use case start

                OpenTrackData Data_Second = new OpenTrackData(LocalPort, ReceiveTimeout);

                if (Data_Second.bLastReceiveSucceed)
                {
                    // Good data
                    Console.Write("Data_Second: X: " + Data_Second.X + ", Y: " + Data_Second.Y + ", z: " + Data_Second.Z + "\n");
                    Console.Write("yaw: " + Data_Second.Yaw + ", pitch: " + Data_Second.Pitch + ", roll: " + Data_Second.Roll + "\n");
                }
                else
                {
                    // Empty data
                    Console.Write("No Data_Second\n");
                }
                */
                // Second use case end, async
                // OR
                // Third use case start, async

                //OpenTrackData Data_Third = new OpenTrackData(new OpenTrackDataCallback(OnReceivedData), LocalPort, ReceiveTimeout);

                // Third use case end, async
                // OR
                // Fourth use case start, async

                OpenTrackData Data_Fourth = new OpenTrackData();

                Data_Fourth.ReceiveOpenTrackDataAsync(new OpenTrackDataCallback(OnReceivedData), LocalPort, ReceiveTimeout);

                // Fourth use case end, async

                Thread.Sleep(1000);
            }
        }
    }
}
