using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_monitor
    {
        public String Name { get; set; }
        public String Availability { get; set; }
        public String Caption { get; set; }
        public String InstallDate { get; set; }
        public String ConfigManagerUserConfig { get; set; }
        public String Description { get; set; }
        public String DeviceID { get; set; }
        public String ErrorCleared { get; set; }
        public String ErrorDescription { get; set; }
        public String LastErrorCode { get; set; }
        public String MonitorManufacturer { get; set; }
        public String PNPDeviceID { get; set; }
        public String MonitorType { get; set; }
        public String PixelsPerXLogicalInch { get; set; }
        public String PixelsPerYLogicalInch { get; set; }
        public String ScreenHeight { get; set; }
        public String ScreenWidth { get; set; }
        public String Status { get; set; }
    }
}
