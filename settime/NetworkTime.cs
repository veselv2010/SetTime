using System;
using System.Net;
using System.Net.Sockets;

namespace SetTime
{
    internal class NetworkTime
    {
        public DateTime CurrentNtp { get; set; }
 
        public NetworkTime()
        {
            GetNetworkTime();
        }

        public DateTime GetNetworkTime()
        {
            const string ntpServer = "1.pool.ntp.org";

            var ntpData = new byte[48];

            ntpData[0] = 0x1B;

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            var ipEndPoint = new IPEndPoint(addresses[0], 123);

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Connect(ipEndPoint);

                socket.ReceiveTimeout = 3000;

                socket.Send(ntpData);
                socket.Receive(ntpData);
            }

            const byte serverReplyTime = 40;

            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);
            
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return CurrentNtp = networkDateTime;
        }

        private uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        public void TimeManipulation(string senderName, UInt16 parsedSec)
        {
            char operation = senderName[senderName.Length - 1];

            var st = new NativeMethods.SYSTEMTIME();
            NativeMethods.GetSystemTime(ref st);

            if (senderName.StartsWith("s"))
            {
                st.wSecond = operation == 'A' ? (ushort)(st.wSecond + parsedSec % 60) :
                                                (ushort)(st.wSecond - parsedSec % 60);
            }
            else
            {
                st.wMilliseconds = operation == 'A' ? (ushort)(st.wMilliseconds + parsedSec % 60000) :
                                                      (ushort)(st.wMilliseconds - parsedSec % 60000);
            }
            NativeMethods.SetSystemTime(ref st);
        }
    }
}
