using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        int[] arr;
        int count ,c,med,Low,Hight,bt,moda;
        double average,standart;
        List<int> bs;
        ClientData clientData;
        bool stopthreadbl;
        uint countLose;
        Thread thread, showdate, updateDate;
        Socket s;
        byte[] b;

        void StopThread()
        {
            stopthreadbl = true;
            thread.Abort();
        }
        void StartThread()
        {
            stopthreadbl = false;
            thread = new Thread(TakeAndSaveByte);
            thread.Start();
            while (!thread.IsAlive)
            {
                Thread.Sleep(1);
            }
        }
        public Client()
        {
            count = 0;
            b = new byte[16];
            
            average = 0;
            clientData = new ClientData();
            clientData.UnSerialize();
            CriateConnection();
            bs = new List<int>();
            showdate = new Thread(ShowInfo);
            showdate.Start();
            s.Receive(b);
            c = ((b[3] & 0xFF) << 24) + ((b[2] & 0xFF) << 16) + ((b[1] & 0xFF) << 8) + (b[0] & 0xFF);
            bs.Add(c);
            bt = ((b[7] & 0xFF) << 24) + ((b[6] & 0xFF) << 16) + ((b[5] & 0xFF) << 8) + (b[4] & 0xFF);
            Low = ((b[11] & 0xFF) << 24) + ((b[10] & 0xFF) << 16) + ((b[9] & 0xFF) << 8) + (b[8] & 0xFF);
            Hight = ((b[15] & 0xFF) << 24) + ((b[14] & 0xFF) << 16) + ((b[13] & 0xFF) << 8) + (b[12] & 0xFF);
            arr = new int[Hight - Low];
            stopthreadbl = false;
            countLose = 1;
            thread = new Thread(TakeAndSaveByte);
            thread.Start();
            updateDate = new Thread(UpdateData);
            updateDate.Start();

            

        }
        void CriateConnection() {
            s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, clientData.MulticastGroupHost);
            s.Bind(ipep);

            IPAddress ip = IPAddress.Parse(clientData.MulticastGroupIP);

            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

        }
        void TakeAndSaveByte()
        {
            while (!stopthreadbl)
            {
                
                Thread.Sleep(clientData.PauseTime);
                s.Receive(b);
                c = ((b[3] & 0xFF) << 24) + ((b[2] & 0xFF) << 16) + ((b[1] & 0xFF) << 8) + (b[0] & 0xFF);
                
                bs.Add(c);
                if (bt != 2147483647)
                {
                    if(((uint)(((b[7] & 0xFF) << 24) + ((b[6] & 0xFF) << 16) + ((b[5] & 0xFF) << 8) + (b[4] & 0xFF) - bt) - 1)!= 0){
                        countLose += (uint)(((b[7] & 0xFF) << 24) + ((b[6] & 0xFF) << 16) + ((b[5] & 0xFF) << 8) + (b[4] & 0xFF) - bt) - 1;
                    }
                }
                else
                {
                    countLose += (uint)(((b[7] & 0xFF) << 24) + ((b[6] & 0xFF) << 16) + ((b[5] & 0xFF) << 8) + (b[4] & 0xFF));
                }
                bt = ((b[7] & 0xFF) << 24) + ((b[6] & 0xFF) << 16) + ((b[5] & 0xFF) << 8) + (b[4] & 0xFF);

            }
        }
        void UpdateData()
        {
            while (true)
            {
                if (bs.Count != 0)
                {
                    StopThread();
                    med = Median();
                    moda = Moda();
                    standart = Standart();
                    average = Average();
                    StartThread();
                    Thread.Sleep(100);
                }
            }
        }
        void ShowInfo()
        {
            
            while (true)
            {

                Console.ReadKey();
                if (bs.Count != 0) 
                {

                    double sd = (double)countLose / (double)(countLose + (uint)bs.Count);
                    Console.WriteLine(@"Average: {0} Standart: {1} Moda: {2} Median: {3} lose packages {4}", average, standart, moda, med, sd);
                    
                }
                else
                    Console.WriteLine("No data received, check if the server application is enabled, and if the IP address and host settings are correct");
            
            }
            
        }
        double AvarageDate(List<int> _bs)
        {
            int sum = 0;
            foreach (int b in _bs)
            {
                sum += b;
            }
            return sum / _bs.Count;
        }
        double AvarageDate(List<int> _bs,int average,int count)
        {
            int sum = 0;
            foreach (int b in _bs)
            {
                sum += b;
            }
            return sum / _bs.Count;
        }
        double StandartDate(List<int> _bs)
        {
            double sum = 0;
            foreach (int b in _bs)
            {
                sum += Math.Pow(b - AvarageDate(_bs), 2);

            }
            return Math.Sqrt(sum / _bs.Count);
        }
        int Fashion(List<int> _bs)
        {
            _bs.Sort();
            int count = 0;
            int _count = 0;
            int ind = 1;
            int last = _bs[0];
            for (int i = 1; i < _bs.Count - 1; i++)
            {
                if (_bs[i] == last)
                {
                    count++;
                }
                else
                {

                    if (count >= _count)
                    {
                        _count = count + 1;
                        ind = i;
                    }
                    count = 0;
                }
                last = _bs[i];

            }
            return _bs[ind - 1];
        }
        int Median()
        {
            bs.Sort();
            if (bs.Count % 2 == 1)
            {
                return bs[(bs.Count + 1) / 2 - 1];
            }
            else
            {
                return (bs[bs.Count / 2 - 1] + bs[bs.Count / 2]) / 2;
            }
        }
        double Average()
        {

            int sum = 0;
            int i = 1;
            for (; i < bs.Count() - count; i++)
            {
                sum += bs[count + i];
            }
            double dsum = (count * average + sum) / bs.Count();
            count = bs.Count();
            if (bs.Count() != 0)
                return dsum;
            else
                return 0;
        }
        double Standart()
        {

            double sum = 0;
            int i = 1;
            for (; i < bs.Count() - count; i++)
            {
                sum += Math.Pow(bs[count + i] - average, 2);
            }
            double dsum = (count * standart + Math.Sqrt(sum)) / bs.Count();
            ;
            if (bs.Count() != 0)
                return dsum;
            else
                return 0;
        }
        int Moda()
        {
            int max = 0;
            int ind = 0;
            int i = 1;
            for (; i < bs.Count() - count; i++)
            {
                arr[bs[count + i] - 1 - Low]++;
            }
            for (int j = 0; j < arr.Length; j++)
            {
                if (arr[j] != 0 && arr[j] >= max)
                {
                    max = arr[j];
                    ind = j + 1 + Low;
                }
            }
            int dsum = ind;
            if (bs.Count() != 0)
                return dsum;
            else
                return 0;
        }

    }
}
