using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_printer
    {
        public String Name { get; set; }
        public Boolean IsNetwork { get; set; }
        public Boolean IsDefaultPrinter { get; set; }
        public String DeviceID { get; set; }
        public String Status { get; set; }
    }
}
