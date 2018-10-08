using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.hosts
{
    public class host_software
    {
        public string Name { get; set; }
        public UInt16 AssignmentType { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string IdentifyingNumber { get; set; }
        public string InstallDate { get; set; }
        public DateTime InstallDate2 { get; set; }
        public string InstallLocation { get; set; }
        public Int16 InstallState { get; set; }
        public string HelpLink { get; set; }
        public string HelpTelephone { get; set; }
        public string InstallSource { get; set; }
        public string Language { get; set; }
        public string LocalPackage { get; set; }
        public string PackageCache { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public string ProductID { get; set; }
        public string RegOwner { get; set; }
        public string RegCompany { get; set; }
        public string SKUNumber { get; set; }
        public string Transforms { get; set; }
        public string URLInfoAbout { get; set; }
        public string URLUpdateInfo { get; set; }
        public string Vendor { get; set; }
        public UInt32 WordCount { get; set; }
        public string Version { get; set; }
    }
}
