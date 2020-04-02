using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace First
{
    public class MyData
    {
        public MyData()
        {
            //this.UnSerialize();
        }
        public string MulticastGroupIP { get; set; }
        public int LowRandomNum { get; set; }
        public int HaightRandomNum { get; set; }
        public int MulticastGroupHost { get; set; }

        public MyData Serialize()
        {
            XmlSerializer xml = new XmlSerializer(typeof(MyData));
            using(FileStream fs = new FileStream("Config.xml",FileMode.OpenOrCreate))
            {
                xml.Serialize(fs, this);
                fs.Close();

            }
            return this;
        }
        public MyData UnSerialize()
        {
            MyData myData;
            XmlSerializer xml = new XmlSerializer(typeof(MyData));
            
            using (FileStream fs = new FileStream("Config.xml", FileMode.Open))
            {
                myData = (MyData)xml.Deserialize(fs);
                fs.Close();

            }
            this.HaightRandomNum = myData.HaightRandomNum;
            this.LowRandomNum = myData.LowRandomNum;
            this.MulticastGroupHost = myData.MulticastGroupHost;
            this.MulticastGroupIP = myData.MulticastGroupIP;
            return this;
        }
    }
}
