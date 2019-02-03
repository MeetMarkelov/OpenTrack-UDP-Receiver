using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OpenTrackUDPReceiver
{
    public delegate void OpenTrackDataCallback(OpenTrackData Data);

    // This class work properly only if OpenTrack 'Output' set as 'UDP over network' 
    // Local port provided to this class should be same as in OpenTrack 
    // IP of machine using this class should be same as in OpenTrack 
    // If this class using on the same machine with OpenTrack use '127.0.0.1' aka 'localhost' 
    public struct OpenTrackData
    {
        //public int ReceiveTimeout;          // Timeout for receive operation 
        public bool bLastReceiveSucceed;    // Was last receive operation succeed 
        public UdpClient UDPClient;
        public OpenTrackDataCallback Callback;

        public double X;
        public double Y;
        public double Z;
        public double Yaw;
        public double Pitch;
        public double Roll;

        // Default constructor, makes OpenTrackData from provided values
        public OpenTrackData(double x = 0, double y = 0, double z = 0, double yaw = 0, double pitch = 0, double roll = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
            this.bLastReceiveSucceed = false;
            this.UDPClient = null;
            this.Callback = null;
        }

        // Static function, parse provided array of bytes into provided OpenTrackData
        public static void BytesToOpenTrackData(ref OpenTrackData Data, byte[] bytes)
        {
            if (bytes.Length != 48)
                return;
            if (BitConverter.IsLittleEndian)
                bytes.Reverse();
            Data.X = BitConverter.ToDouble(bytes, 0);
            Data.Y = BitConverter.ToDouble(bytes, 8);
            Data.Z = BitConverter.ToDouble(bytes, 16);
            Data.Yaw = BitConverter.ToDouble(bytes, 24);
            Data.Pitch = BitConverter.ToDouble(bytes, 32);
            Data.Roll = BitConverter.ToDouble(bytes, 40);
        }

        // Static function, fill provided OpenTrackData with values from OpenTrack, return true is OpenTrackData filled properly, false otherwise
        public static bool GetOpenTrackData(ref OpenTrackData Data, int LocalPort, int ReceiveTimeout = 5000)
        {
            try
            {
                Data.UDPClient = new UdpClient(LocalPort);
                Data.UDPClient.Client.ReceiveTimeout = ReceiveTimeout;
                IPEndPoint ep = null;
                Byte[] receivedData = Data.UDPClient.Receive(ref ep);              
                BytesToOpenTrackData(ref Data, receivedData);
                Data.bLastReceiveSucceed = true;
            }
            catch (Exception/* e*/)
            {
                //Console.WriteLine(e.ToString());
                Data.bLastReceiveSucceed = false;
            }
            Data.UDPClient.Close();
            return Data.bLastReceiveSucceed;
        }

        // Constructor, make OpenTrackData from provided array of bytes
        public OpenTrackData(byte[] bytes)
        {
            this.X = this.Y = this.Z = this.Yaw = this.Pitch = this.Roll = 0;
            this.bLastReceiveSucceed = false;
            this.UDPClient = null;
            this.Callback = null;
            BytesToOpenTrackData(ref this, bytes);
        }

        // Constructor, make OpenTrackData with values from OpenTrack
        public OpenTrackData(int LocalPort, int ReceiveTimeout = 5000)
        {
            this.X = this.Y = this.Z = this.Yaw = this.Pitch = this.Roll = 0;
            this.bLastReceiveSucceed = false;
            this.UDPClient = null;
            this.Callback = null;
            GetOpenTrackData(ref this, LocalPort, ReceiveTimeout);
        }

        // Functions that fires when received OpenTrack data
        public void OnDataReceived(IAsyncResult res)
        {
            if (!res.IsCompleted)
                return;
            IPEndPoint ep = null;
            BytesToOpenTrackData(ref this, UDPClient.EndReceive(res, ref ep));
            UDPClient.Close();
            try
            {
                this.Callback.Invoke(this);
                bLastReceiveSucceed = true;
            }
            catch (Exception)
            { }
        }

        // Async function, start receiving, fires Callback function when receive was properly finished
        public void GetOpenTrackDataAsync(OpenTrackDataCallback Callback, int LocalPort, int ReceiveTimeout = 5000)
        {
            this.Callback = Callback;
            this.bLastReceiveSucceed = false;
            try
            {
                UDPClient = new UdpClient(LocalPort);
                UDPClient.Client.ReceiveTimeout = ReceiveTimeout;
            }
            catch (Exception/* e*/)
            {
                //Console.WriteLine(e.ToString());
            }
            try
            {
                UDPClient.BeginReceive(new AsyncCallback(OnDataReceived), null);
                
            }
            catch (Exception/* e*/)
            {
                //Console.WriteLine(e.ToString());
            }
        }

        // Async constructor, start receiving, fires Callback function when receive was properly finished
        public OpenTrackData(OpenTrackDataCallback Callback, int LocalPort, int ReceiveTimeout = 5000)
        {
            this.X = this.Y = this.Z = this.Yaw = this.Pitch = this.Roll = 0;
            this.bLastReceiveSucceed = false;
            this.UDPClient = null;
            this.Callback = null;
            GetOpenTrackDataAsync(Callback, LocalPort, ReceiveTimeout);
        }
    };

}
