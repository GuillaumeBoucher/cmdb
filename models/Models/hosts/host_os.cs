using System;

namespace model.hosts
{
    public class host_os 
    {
        public String name { get; set; }
        public Boolean Is64Bit { get; set; }
        public String version { get; set; }
        public String WindowsDirectory { get; set; }
        public String SystemDirectory { get; set; }
        public int CountryCode { get; set; }
        public String famille { get; set; }
    }
}
