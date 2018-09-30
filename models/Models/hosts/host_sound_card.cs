using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_sound_card
    {
       public String Name { get; set; }
       public String ProductName { get; set; }
       public String DeviceID { get; set; }
       public Boolean PowerManagementSupported { get; set; }
       public String Status { get; set; }

    }
}
