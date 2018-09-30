using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace model.hosts
{
    public class host_ipadress
    {
        //constructeur 
        
        public String IPAddress { get; set;}
        public String IPAddressVersion { get; set; }
        public String MTU { get; set; }
        public String NetMask { get; set; }
        public String CidrNetMask { get; set; }
        public String TypeCSA { get; set; }
        public bool IsStaticIP { get; set; }
    }
}
