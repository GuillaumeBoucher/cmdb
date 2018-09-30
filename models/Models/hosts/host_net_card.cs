using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_net_card
    {
        //constructeur
        public host_net_card()
        {
            this.ListIP = new List<host_ipadress>();
            
        }
        public String Name { get; set; }
        public String @Type { get; set; }
        public String Description { get; set; }
        public String AdressMAC { get; set; }
        public String Status { get; set; }
        public String Speed { get; set; }
        public List<host_ipadress> ListIP { get; set;}
        
    }
}
