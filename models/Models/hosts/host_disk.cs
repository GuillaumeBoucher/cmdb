using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.hosts
{
    public class host_disk
    {
        public String Name { get; set; }
        public String DriveType { get; set; }
        public String VolumeLabel { get; set; }
        public String FileSystem  { get; set; }
        public String TotalAvailableSpace { get; set; }
        public String TotalSizeOfDrive { get; set; }


    }
}
