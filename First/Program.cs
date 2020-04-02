using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace First
{
    class Program
    {
        static void Main(string[] args)
        {
            MyData myData = new MyData();

            myData.UnSerialize();
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress ip = IPAddress.Parse(myData.MulticastGroupIP);

            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));

            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);

            IPEndPoint ipep = new IPEndPoint(ip, myData.MulticastGroupHost);

            s.Connect(ipep);
            Random rd = new Random();
            List<int> bss = new List<int>();
            byte[] b = new byte[16];
            int inum = 0;
            while (true)
            {
                
                int a = (int)(rd.Next(myData.LowRandomNum,myData.HaightRandomNum));
                b[0] = (byte)a;
                b[1] = (byte)(a>>8);
                b[2] = (byte)(a>>16);
                b[3] = (byte)(a>>24);                
                b[4] = (byte)inum;
                b[5] = (byte)(inum >> 8);
                b[6] = (byte)(inum >> 16);
                b[7] = (byte)(inum >> 24);
                b[8] = (byte)myData.LowRandomNum;
                b[9] = (byte)(myData.LowRandomNum >> 8);
                b[10] = (byte)(myData.LowRandomNum >> 16);
                b[11] = (byte)(myData.LowRandomNum >> 24);
                b[12] = (byte)myData.HaightRandomNum;
                b[13] = (byte)(myData.HaightRandomNum >> 8);
                b[14] = (byte)(myData.HaightRandomNum >> 16);
                b[15] = (byte)(myData.HaightRandomNum >> 24);
                bss.Add(a);
                
                s.Send(b, b.Length, SocketFlags.None);
                if (inum == 2147483647)
                    inum = 0;
                else
                    inum++;
            }
        }
    }
}
