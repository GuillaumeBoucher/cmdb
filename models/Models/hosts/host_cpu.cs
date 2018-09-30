using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_cpu
    {
        public String Name { get; set; }
        public String Famille { get; set; }
        public int NumberOfLogicalProcessors { get; set; }
        public int NumberOfCores  { get; set; }
        public int CurrentClockSpeed { get; set; }
        public int NombreTotalDeCPU { get; set; }


        //String Name = obj["Name"].ToString();
        //String DeviceID = obj["DeviceID"].ToString();
        //String Manufacturer = obj["Manufacturer"].ToString();
        //String Caption = obj["Caption"].ToString();
        //String Architecture = obj["Architecture"].ToString();
        //String Family = obj["Family"].ToString();
        //String ProcessorType = obj["ProcessorType"].ToString();
        //String AddressWidth = obj["AddressWidth"].ToString();

    }
}
