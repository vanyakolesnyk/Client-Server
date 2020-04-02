using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client
{
    public class ClientData
    {
        public int PauseTime { get; set; }
        public string MulticastGroupIP { get; set; }
        public int MulticastGroupHost { get; set; }

        public ClientData Serialize()
        {
            XmlSerializer xml = new XmlSerializer(typeof(ClientData));
            using (FileStream fs = new FileStream("ConfigClient.xml", FileMode.OpenOrCreate))
            {
                xml.Serialize(fs, this);
                fs.Close();

            }
            return this;
        }
        public ClientData UnSerialize()
        {
            ClientData myData;
            XmlSerializer xml = new XmlSerializer(typeof(ClientData));

            using (FileStream fs = new FileStream("ConfigClient.xml", FileMode.Open))
            {
                myData = (ClientData)xml.Deserialize(fs);

            }
            this.PauseTime = myData.PauseTime;
            this.MulticastGroupHost = myData.MulticastGroupHost;
            this.MulticastGroupIP = myData.MulticastGroupIP;
            return this;
        }
    }
}
