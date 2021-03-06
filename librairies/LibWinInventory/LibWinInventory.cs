﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Diagnostics;
using NLog;
using model.hosts;
using System.Management;
using System.DirectoryServices;
using System.Text;
using System.Security.Principal;
using XperiCode.Impersonator;

namespace lib.WinInventory
{
    public class LibWinInventory
    {
        #region private_property
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private host MyHost { get; set; }
        private readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        #endregion

        #region private_method
        
        private List<string> GetLocalUser()
        {
            List<string> _ret = new List<string>();

            try
            {
                string host_name = Environment.MachineName.ToLower();
                SelectQuery sQuery = new SelectQuery("Win32_UserAccount", "Domain='" + host_name + "'");

                ManagementObjectSearcher aManagementObjectSearcher = new ManagementObjectSearcher(sQuery);

                //ManagementObjectCollection représente différentes collections d'objets de gestion extraits via WMI. 
                ManagementObjectCollection objCollection = aManagementObjectSearcher.Get();

                //int aaaa = objCollection.Count;

                //ManagementObjectCollection objCollection = myProcessorObject.Get();
                if (objCollection.Count > 0)
                {
                    foreach (ManagementObject obj in objCollection)
                    {
                        //UInt32 AccountType = this.GetProperties<UInt32>(obj, "AccountType");
                        //string Caption = this.GetProperties<string>(obj, "Caption").ToLower();
                        //string Description = this.GetProperties<string>(obj, "Description").ToLower();
                        //Boolean Disabled = this.GetProperties<Boolean>(obj, "Disabled");
                        //string Domain = this.GetProperties<string>(obj, "Domain").ToLower();
                        //string FullName = this.GetProperties<string>(obj, "FullName").ToLower();
                        //DateTime InstallDate = this.GetProperties<DateTime>(obj, "InstallDate");
                        //Boolean LocalAccount = this.GetProperties<Boolean>(obj, "LocalAccount");
                        //Boolean Lockout = this.GetProperties<Boolean>(obj, "Lockout");
                        string Name = this.GetProperties<string>(obj, "Name").ToLower();
                        //Boolean PasswordChangeable = this.GetProperties<Boolean>(obj, "PasswordChangeable");
                        //Boolean PasswordExpires = this.GetProperties<Boolean>(obj, "PasswordExpires");
                        //Boolean PasswordRequired = this.GetProperties<Boolean>(obj, "PasswordRequired");
                        //string SID = this.GetProperties<string>(obj, "Name").ToLower();
                        //UInt32 SIDType = this.GetProperties<UInt32>(obj, "SIDType");
                        //string Status = this.GetProperties<string>(obj, "Status").ToLower();

                        if (Name.Length > 0)
                        {
                            _ret.Add(Name);
                        }
                        //string grp = this.GetGroupsForUser(host_name, Name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetLocalUser");
            }

            return _ret;
        }

        private void LoadInstalledPrograms()
        {
            List<string> installedPrograms = new List<string>();
        
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
        
            ManagementObjectCollection managementObjectCollection = mos.Get();
        
            foreach (ManagementObject mo in managementObjectCollection)
            {
                installedPrograms.Add(mo["Name"].ToString());
            }
        
            Console.WriteLine("Length - {0}", installedPrograms.Count);
        }

        private void GetSoftware()
        {
           try
           {               

                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");

                //ManagementObjectCollection représente différentes collections d'objets de gestion extraits via WMI. 
                ManagementObjectCollection objCollection = mos.Get();

               // int aaaa = objCollection.Count;

                //ManagementObjectCollection objCollection = myProcessorObject.Get();
                if (objCollection.Count > 0)
                {
                    foreach (ManagementObject obj in objCollection)
                    {

                        host_software _newSW = new host_software();

                        _newSW.Name = this.GetProperties<string>(obj, "Name").ToLower();
                        _newSW.AssignmentType = this.GetProperties<UInt16>(obj, "AssignmentType");
                        _newSW.Caption = this.GetProperties<string>(obj, "Caption").ToLower();
                        _newSW.Description = this.GetProperties<string>(obj, "Description").ToLower();
                        _newSW.IdentifyingNumber = this.GetProperties<string>(obj, "IdentifyingNumber").ToLower();
                        _newSW.InstallDate = this.GetProperties<string>(obj, "InstallDate").ToLower();
                        _newSW.InstallDate2 = this.GetProperties<DateTime>(obj, "InstallDate2");
                        _newSW.InstallLocation = this.GetProperties<string>(obj, "InstallLocation").ToLower();
                        _newSW.InstallState = this.GetProperties<Int16>(obj, "InstallState");
                        _newSW.HelpLink = this.GetProperties<string>(obj, "HelpLink").ToLower();
                        _newSW.HelpTelephone = this.GetProperties<string>(obj, "HelpTelephone").ToLower();
                        _newSW.InstallSource = this.GetProperties<string>(obj, "InstallSource").ToLower();
                        _newSW.Language = this.GetProperties<string>(obj, "Language").ToLower();
                        _newSW.LocalPackage = this.GetProperties<string>(obj, "LocalPackage").ToLower();
                        _newSW.PackageCache = this.GetProperties<string>(obj, "PackageCache").ToLower();
                        _newSW.PackageCode = this.GetProperties<string>(obj, "PackageCode").ToLower();
                        _newSW.PackageName = this.GetProperties<string>(obj, "PackageName").ToLower();
                        _newSW.ProductID = this.GetProperties<string>(obj, "ProductID").ToLower();
                        _newSW.RegOwner = this.GetProperties<string>(obj, "RegOwner").ToLower();
                        _newSW.RegCompany = this.GetProperties<string>(obj, "RegCompany").ToLower();
                        _newSW.SKUNumber = this.GetProperties<string>(obj, "SKUNumber").ToLower();
                        _newSW.Transforms = this.GetProperties<string>(obj, "Transforms").ToLower();
                        _newSW.URLInfoAbout = this.GetProperties<string>(obj, "URLInfoAbout").ToLower();
                        _newSW.URLUpdateInfo = this.GetProperties<string>(obj, "URLUpdateInfo").ToLower();
                        _newSW.Vendor = this.GetProperties<string>(obj, "Vendor").ToLower();
                        _newSW.WordCount = this.GetProperties<UInt32>(obj, "WordCount");
                        _newSW.Version = this.GetProperties<string>(obj, "Version").ToLower();

                        if (_newSW.Name.Length > 0)
                        {
                            this.MyHost.softwares.Add(_newSW);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetLocalUser");
            }

           
        }

        private IEnumerable<string> GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            Queue<string> pending = new Queue<string>();
            pending.Enqueue(rootFolderPath);
            string[] tmp;
            while (pending.Count > 0)
            {
                rootFolderPath = pending.Dequeue();
                try
                {
                    tmp = Directory.GetFiles(rootFolderPath, fileSearchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                for (int i = 0; i < tmp.Length; i++)
                {
                    yield return tmp[i];
                }
                tmp = Directory.GetDirectories(rootFolderPath);
                for (int i = 0; i < tmp.Length; i++)
                {
                    pending.Enqueue(tmp[i]);
                }
            }
        }

        private List<string> GetLocalGroup()
        {
            List<string> _ret = new List<string>();

            try
            {
                string host_name = Environment.MachineName.ToLower();
                SelectQuery sQuery = new SelectQuery("Win32_Group", "Domain='" + host_name + "'");

                ManagementObjectSearcher aManagementObjectSearcher = new ManagementObjectSearcher(sQuery);

                //ManagementObjectCollection représente différentes collections d'objets de gestion extraits via WMI. 
                ManagementObjectCollection objCollection = aManagementObjectSearcher.Get();

                //int aaaa = objCollection.Count;

                //ManagementObjectCollection objCollection = myProcessorObject.Get();
                if (objCollection.Count > 0)
                {
                    foreach (ManagementObject obj in objCollection)
                    {
                                               

                        //string Caption = this.GetProperties<string>(obj, "Caption").ToLower();
                        //string Description = this.GetProperties<string>(obj, "Description").ToLower();
                        //DateTime InstallDate = this.GetProperties<DateTime>(obj, "InstallDate");
                        //string Status = this.GetProperties<string>(obj, "Caption").ToLower();
                        //Boolean LocalAccount = this.GetProperties<Boolean>(obj, "LocalAccount");
                        //string SID = this.GetProperties<string>(obj, "SID").ToLower();
                        //uint SIDType = this.GetProperties<uint>(obj, "SIDType");
                        //string Domain = this.GetProperties<string>(obj, "Domain").ToLower();
                        string Name = this.GetProperties<string>(obj, "Name").ToLower();
                        if (Name.Length > 0)
                        {
                            _ret.Add(Name);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetLocalUser");
            }

            return _ret;
        }

        private List<string> GetUsersInGroup(string group)
        {
            List<string> _ret = new List<string>();

                       


            string sFromWhere = "LDAP://WM2008R2ENT:389/dc=dom,dc=fr";
            DirectoryEntry deBase = new DirectoryEntry(sFromWhere, "dom\\jpb", "test.2011");

            /* To find all the users member of groups "Grp1"  :
             * Set the base to the groups container DN; for example root DN (dc=societe,dc=fr) 
             * Set the scope to subtree
             * Use the following filter :
             * (member:1.2.840.113556.1.4.1941:=CN=Grp1,OU=MonOu,DC=X)
             * coupled with LDAP_MATCHING_RULE_BIT_AND on userAccountControl with ACCOUNTDISABLE
             */
            DirectorySearcher dsLookFor = new DirectorySearcher(deBase);
            dsLookFor.Filter = "(&(memberof:1.2.840.113556.1.4.1941:=CN=MonGrpSec,OU=MonOu,DC=dom,DC=fr)(userAccountControl:1.2.840.113556.1.4.803:=2))";
            dsLookFor.SearchScope = SearchScope.Subtree;
            dsLookFor.PropertiesToLoad.Add("cn");

            SearchResultCollection srcUsers = dsLookFor.FindAll();

            /* Just to know if user is present in an other group
             */
            foreach (SearchResult srcUser in srcUsers)
            {
                Console.WriteLine("{0}", srcUser.Path);
            }

            return _ret;
        }

        private string GetGroupsForUser(string domain,string UserName)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_GroupUser where PartComponent=\"Win32_UserAccount.Domain='"+domain+"',Name='" + UserName + "'\"");
            StringBuilder strGroups = new StringBuilder();

            foreach (ManagementObject mObject in searcher.Get())
            {
                ManagementPath path = new ManagementPath(mObject["GroupComponent"].ToString());

                if (path.ClassName == "Win32_Group")
                {
                    String[] names = path.RelativePath.Split(',');
                    strGroups.Append(names[1].Substring(names[1].IndexOf("=") + 1).Replace('"', ' ').Trim() + ", ");
                }
            }
            return strGroups.ToString();
        }

        private string GetGroupsForCurentDomainUser()
        {
            string domain = "csa.lan";
            string UserName = Environment.UserName;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_GroupUser where PartComponent=\"Win32_UserAccount.Domain='" + domain + "',Name='" + UserName + "'\"");
            StringBuilder strGroups = new StringBuilder();

            foreach (ManagementObject mObject in searcher.Get())
            {
                ManagementPath path = new ManagementPath(mObject["GroupComponent"].ToString());

                if (path.ClassName == "Win32_Group")
                {
                    String[] names = path.RelativePath.Split(',');
                    strGroups.Append(names[1].Substring(names[1].IndexOf("=") + 1).Replace('"', ' ').Trim() + ", ");
                }
            }
            return strGroups.ToString();
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public T GetProperties<T>(ManagementObject obj,string prop_name) where T : IConvertible
        {
            T _ret;

            try
            {   var value = obj[prop_name];
                var TypeStr = typeof(T).Name.ToLower();

                if(value is null && TypeStr == "string")
                {
                    string b = "";
                    _ret = (T)Convert.ChangeType(b, typeof(T));
                }
                else
                {
                    _ret = (T)Convert.ChangeType(value, typeof(T));
                }
            }
            catch
            {
                //var a = typeof(T).Name.ToLower();
                switch (typeof(T).Name.ToLower())
                {
                    case "string":
                        {
                            string b = "";
                            _ret = (T)Convert.ChangeType(b, typeof(T));
                            break;
                        }
                    case "datetime":
                        {
                            DateTime b = new DateTime();
                            _ret = (T)Convert.ChangeType(b, typeof(T));
                            break;
                        }
                    case "nullable`1":
                    {
                            _ret = default(T);
                    break;
                    }
                    case "boolean":
                        {
                            Boolean? b = null;
                            _ret = (T) Convert.ChangeType(b, typeof(T));
                            break;
                        }
                    case "int32":
                        {
                            _ret = (T)Convert.ChangeType(-1, typeof(T));
                            break;
                        }
                    default:
                        {
                            _ret = (T)Convert.ChangeType("", typeof(T));
                            break;
                        }
                }
             }
            return _ret;
        }
        private void getOS()
        {
            try
            {
                ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                foreach (ManagementObject obj in myOperativeSystemObject.Get())
                {
                    this.MyHost.os.name = this.GetProperties<string>(obj, "Caption").ToLower();
                    logger.Info("os : "+this.MyHost.os.name);
                    if (this.MyHost.os.name.Contains("windows"))
                    {
                        this.MyHost.os.famille = "windows";
                    }
                    this.MyHost.SerialNumber = this.GetProperties<string>(obj, "SerialNumber").ToLower();
                    this.MyHost.os.version = this.GetProperties<string>(obj, "Version").ToLower();
                    this.MyHost.os.Is64Bit = Environment.Is64BitOperatingSystem;
                    this.MyHost.os.WindowsDirectory = this.GetProperties<string>(obj, "WindowsDirectory").ToLower();
                    this.MyHost.os.SystemDirectory = this.GetProperties<string>(obj, "SystemDirectory").ToLower();
                    this.MyHost.os.CountryCode = this.GetProperties<Int32>(obj, "CountryCode");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "os : exception msg=");
            }
        }
        private void getCPU()
        {
            try
            { 
                ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
                ManagementObjectCollection objCollection = myProcessorObject.Get();
                if (objCollection.Count > 0)
                {   
                    ManagementObject obj = objCollection.OfType<ManagementObject>().First();
                    this.MyHost.cpu.Name = this.GetProperties<string>(obj,"Name").ToLower();
                    logger.Info("cpu: " + this.MyHost.cpu.Name);
                    this.MyHost.cpu.NumberOfCores = this.GetProperties<Int32>(obj,"NumberOfCores");
                    this.MyHost.cpu.NumberOfLogicalProcessors = this.GetProperties<Int32>(obj,"NumberOfLogicalProcessors");
                    this.MyHost.cpu.NombreTotalDeCPU = this.MyHost.cpu.NumberOfCores * this.MyHost.cpu.NumberOfLogicalProcessors;
                    this.MyHost.cpu.CurrentClockSpeed = this.GetProperties<Int32>(obj,"CurrentClockSpeed");
                    logger.Info("cpu: "+ this.MyHost.cpu.NumberOfCores+" core x"+ this.MyHost.cpu.NumberOfLogicalProcessors + "=" + this.MyHost.cpu.NombreTotalDeCPU);
                    this.MyHost.cpu.Famille = this.GetProperties<string>(obj, "Caption").ToLower();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "getOS");
            }
}
        private void getDISK()
        {
            try { 
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                host_disk _newDisk = new host_disk();
                _newDisk.Name = d.Name;
                _newDisk.DriveType = d.DriveType.ToString();
                if (d.IsReady == true)
                {
                    _newDisk.VolumeLabel = d.VolumeLabel;
                    _newDisk.FileSystem = d.DriveFormat;
                    _newDisk.TotalAvailableSpace = this.SizeSuffix(d.TotalFreeSpace);
                    _newDisk.TotalSizeOfDrive = this.SizeSuffix(d.TotalSize);
                }
                
                logger.Info("disk: " + _newDisk.DriveType + " " + _newDisk.Name + " ["+ _newDisk.TotalSizeOfDrive+"]");

                this.MyHost.disk.Add(_newDisk);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "disk :");
            }
        }
        private string SizeSuffix(Int64 value, bool IsForNetwork = false)
        {
            string _ret = "";
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));


            if (IsForNetwork)
            {
                int round_adjustedSize = Convert.ToInt32(Math.Round(adjustedSize));
                if (round_adjustedSize == 9) { round_adjustedSize = 10; }

                _ret = string.Format("{0} {1}", round_adjustedSize, SizeSuffixes[mag]);

            }
            else
            {
                _ret = string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
            }
            return _ret;

        }
        private void getVIDEO()
        {
            try
            {
                ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (ManagementObject obj in myVideoObject.Get())
                {
                    host_video_card _newVidCard = new host_video_card();
                    _newVidCard.Name = this.GetProperties<string>(obj,"Name").ToLower();
                    _newVidCard.DriverVersion = this.GetProperties<string>(obj,"DriverVersion").ToLower();                
                    _newVidCard.VideoProcessor = this.GetProperties<string>(obj, "VideoProcessor").ToLower();
                    this.MyHost.video_Cards.Add(_newVidCard);
                    logger.Info("video: " + _newVidCard.Name);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "getOS");
            }
}
        private void GetMonitor()
        {
            try
            {
                SelectQuery Sq = new SelectQuery("Win32_DesktopMonitor");
                ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
                ManagementObjectCollection osDetailsCollection = objOSDetails.Get();
                foreach (ManagementObject obj in osDetailsCollection)
                {
                    host_monitor _NewMonitor = new host_monitor();
                    _NewMonitor.Name = this.GetProperties<string>(obj, "Name").ToLower();
                    _NewMonitor.Availability = this.GetProperties<string>(obj, "Availability").ToLower();
                    _NewMonitor.Caption = this.GetProperties<string>(obj, "Caption").ToLower();
                    _NewMonitor.InstallDate = this.GetProperties<DateTime>(obj, "InstallDate").ToString().ToLower();
                    _NewMonitor.ConfigManagerUserConfig = this.GetProperties<string>(obj, "ConfigManagerUserConfig").ToLower();
                    _NewMonitor.Description = this.GetProperties<string>(obj, "Description").ToLower();
                    _NewMonitor.DeviceID = this.GetProperties<string>(obj, "DeviceID").ToLower();
                    _NewMonitor.ErrorCleared = this.GetProperties<string>(obj, "ErrorCleared").ToLower();
                    _NewMonitor.ErrorDescription = this.GetProperties<string>(obj, "ErrorDescription").ToLower();
                    _NewMonitor.ConfigManagerUserConfig = this.GetProperties<string>(obj, "ConfigManagerUserConfig").ToLower();
                    _NewMonitor.LastErrorCode = this.GetProperties<string>(obj, "LastErrorCode").ToLower();
                    _NewMonitor.MonitorManufacturer = this.GetProperties<string>(obj, "MonitorManufacturer").ToLower();
                    _NewMonitor.PNPDeviceID = this.GetProperties<string>(obj, "PNPDeviceID").ToLower();
                    _NewMonitor.MonitorType = this.GetProperties<string>(obj, "MonitorType").ToLower();
                    _NewMonitor.PixelsPerXLogicalInch = this.GetProperties<string>(obj, "PixelsPerXLogicalInch").ToLower();
                    _NewMonitor.PixelsPerYLogicalInch = this.GetProperties<string>(obj, "PixelsPerYLogicalInch").ToLower();
                    _NewMonitor.ScreenHeight = this.GetProperties<string>(obj, "ScreenHeight").ToLower();
                    _NewMonitor.ScreenWidth = this.GetProperties<string>(obj, "ScreenWidth").ToLower();
                    _NewMonitor.Status = this.GetProperties<string>(obj, "Status").ToLower();

                    logger.Info(String.Format("Monitor: {0} {1}x{2} ({3})", _NewMonitor.Name, _NewMonitor.PixelsPerXLogicalInch, _NewMonitor.PixelsPerYLogicalInch, _NewMonitor.Status));

                    

                    this.MyHost.Monitors.Add(_NewMonitor);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Monitor:");
            }
        }
    
        private List<host_ipadress> GetIPAddresses(NetworkInterface adapter,ref string loglevel, ref string logmessage)
        {
            loglevel = "info";
            logmessage = logmessage + "";
            List<host_ipadress> _ret = new List<host_ipadress>();

            IPInterfaceProperties properties = adapter.GetIPProperties();
            UnicastIPAddressInformationCollection All_uniCast_IP = properties.UnicastAddresses;

            foreach (UnicastIPAddressInformation ip in All_uniCast_IP)
            {
                host_ipadress _newHost_IP = new host_ipadress();

                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    _newHost_IP.IPAddress = ip.Address.ToString();
                    _newHost_IP.IPAddressVersion = "IpV4";
                    string n = adapter.Name.ToLower();
                    _newHost_IP.TypeCSA = this.GetCsaNetworkType(n, _newHost_IP.IPAddress,ref loglevel, ref logmessage);
                    _newHost_IP.NetMask = ip.IPv4Mask.ToString();
                    _newHost_IP.CidrNetMask = ip.PrefixLength.ToString();
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    _newHost_IP.MTU = ipv4.Mtu.ToString().ToLower();
                    _newHost_IP.IsStaticIP = (!ipv4.IsDhcpEnabled);

                    //mauvais netmask
                    if (_newHost_IP.TypeCSA == "lan_serveur" && _newHost_IP.CidrNetMask == "16")
                    {
                        loglevel = "warn";
                        logmessage = "le NetMask n'est pas 255.255.128.0";
                        host_recommandations reco = new host_recommandations();
                        reco.Type = loglevel;
                        reco.Details = logmessage;
                        this.MyHost.Recommandations.Add(reco);
                    }

                }
                if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _newHost_IP.IPAddress = ip.Address.ToString();
                    _newHost_IP.IPAddressVersion = "IpV6";
                    string n = adapter.Name.ToLower();
                    _newHost_IP.TypeCSA = this.GetCsaNetworkType(n, _newHost_IP.IPAddress,ref loglevel,ref logmessage);

                    //APIPA
                    if (_newHost_IP.IPAddress.StartsWith("fe80") || _newHost_IP.IPAddress.StartsWith("169.254"))
                    {
                        loglevel = "warn";
                        if (!logmessage.Contains("apipa"))
                        {
                            logmessage = logmessage + "+apipa";
                        }                        
                    }


                    _newHost_IP.NetMask = ip.IPv4Mask.ToString();
                    _newHost_IP.CidrNetMask = ip.PrefixLength.ToString();
                    IPv6InterfaceProperties ipv6 = properties.GetIPv6Properties();
                    _newHost_IP.MTU = ipv6.Mtu.ToString().ToLower();
                    _newHost_IP.IsStaticIP = false;
                    loglevel = "warn";
                    if(!logmessage.Contains("ipv6"))
                    { 
                        logmessage = logmessage + "+ipv6";
                    }
                }

                _ret.Add(_newHost_IP);
            }
            return _ret;
        }
        private String GetCsaNetworkType(string card_name, string ip,ref string loglevel, ref string logmessage)
        {
            string _ret = "unknown";
            
            //lan_serveur
            if (ip.StartsWith("172.16.3."))
            {
                _ret = "lan_serveur";
                if (!logmessage.Contains("lan_serveur"))
                {
                    logmessage = logmessage + "+lan_serveur";
                }
            }
            //vpn_guillaume
            if (ip.StartsWith("172.18.6."))
            {
                _ret = "vpn_guillaume";
                if (!logmessage.Contains("vpn_guillaume"))
                {
                    logmessage = logmessage + "+vpn_guillaume";
                }
            }
            if (_ret == "unknown")
            {
                loglevel = "warn";
                if (!logmessage.Contains("carteInconnue"))
                {
                    logmessage = logmessage + "+carteInconnue";
                }
            }
            return _ret;
        }



        private string GetIpv4_Ipv6_Support(NetworkInterface adapter)
        {
            string _ret = "";

            // Create a display string for the supported IP versions.
            if (adapter.Supports(NetworkInterfaceComponent.IPv4))
            {
                _ret = "IPv4";
            }
            if (adapter.Supports(NetworkInterfaceComponent.IPv6))
            {
                if (_ret.Length > 0)
                {
                    _ret += " ";
                }
                _ret += "IPv6";
            }

            return _ret;

        }
        private void getNETWORK()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            if (!(nics is null))
            {
                if (nics.Length > 0)
                {
                    foreach (NetworkInterface adapter in nics)
                    {
                        host_net_card _newNetCard = new host_net_card();
                        string loglevel = "info";
                        string logmessage = "";
                        _newNetCard.Type = adapter.NetworkInterfaceType.ToString().ToLower();
                        // on ne prend pas en compte les interfaces loopback
                        if (_newNetCard.Type != "loopback")
                        {
                            _newNetCard.Name = adapter.Name;
                            _newNetCard.Description = adapter.Description;
                            var s = adapter.GetPhysicalAddress().ToString();
                            // add ':' tous les 2 caractères                                    
                            var list = Enumerable
                                .Range(0, s.Length / 2)
                                .Select(i => s.Substring(i * 2, 2))
                                .ToList();
                            _newNetCard.AdressMAC = string.Join(":", list);
                            _newNetCard.Status = adapter.OperationalStatus.ToString();
                            _newNetCard.Speed = this.SizeSuffix(adapter.Speed, true);
                            _newNetCard.ListIP = this.GetIPAddresses(adapter, ref loglevel, ref logmessage);
                            // log
                            string Fisrt_Message = "netcard: " + _newNetCard.Name;
                            string Second_Message = "";
                            int ipNum = 0;
                            foreach (host_ipadress ip in _newNetCard.ListIP)
                            {
                                ipNum++;
                                Second_Message = Second_Message + " " + string.Format("ip{0}={1}", ipNum.ToString(),ip.IPAddress);
                            }
                            if (logmessage.Length > 0)
                            {
                                logmessage = logmessage.Substring(1, logmessage.Length - 1);
                                logmessage = "(" + logmessage + ")";
                            }
                            logmessage = Fisrt_Message + " " + Second_Message + " " +logmessage;
                            
                            host_recommandations reco = new host_recommandations();
                            reco.Type = loglevel;
                            reco.Details = logmessage;
                            this.MyHost.Recommandations.Add(reco);

                            switch (loglevel)
                            {
                                case "info":
                                    {
                                        logger.Info(logmessage);
                                        break;
                                    }
                                case "warn":
                                    {
                                        logger.Warn(logmessage);
                                        break;
                                    }
                                case "error":
                                    {
                                        logger.Error(logmessage);
                                        break;
                                    }
                            }
                            //add inferface
                            this.MyHost.net_Cards.Add(_newNetCard);
                        }

                        //    case "tunnel":
                        //            if (n.Contains("isatap") || d.Contains("isatap") || n.Contains("6to4") || d.Contains("6to4") || n.Contains("teredo") || d.Contains("teredo"))
                        //            {
                        //                string advice = "Lancer les commandes ci-dessous :" + Environment.NewLine;
                        //                advice = advice + " netsh int ipv6 isatap set state disabled" + Environment.NewLine;
                        //                advice = advice + " netsh int ipv6 6to4 set state disabled" + Environment.NewLine;
                        //                advice = advice + " netsh interface teredo set state disable" + Environment.NewLine;
                        //                advice = advice + " netsh interface isatap set state disable" + Environment.NewLine;
                                           
                      
                    }
                }
            }
        }
        private void getPRINTER()
        {
            try
            {
                
                    ManagementObjectSearcher myPrinterObject = new ManagementObjectSearcher("select * from Win32_Printer");
                    ManagementObjectCollection objCol = myPrinterObject.Get();

                    foreach (ManagementObject obj in objCol)
                    {
                        host_printer _newPrinter = new host_printer();
                        _newPrinter.Name = this.GetProperties<string>(obj, "Name").ToLower();
                        _newPrinter.IsNetwork = this.GetProperties<Boolean>(obj, "Network");
                        _newPrinter.IsDefaultPrinter = this.GetProperties<Boolean>(obj, "Default");
                        _newPrinter.DeviceID = this.GetProperties<string>(obj, "DeviceID").ToLower();
                        _newPrinter.Status = this.GetProperties<string>(obj, "Status").ToLower();

                        string logmessage = "";
                        if (_newPrinter.IsDefaultPrinter)
                        {
                            logmessage = "(default) " + _newPrinter.Name;
                        }
                        else
                        {
                            logmessage = _newPrinter.Name;
                        }
                        logger.Info("Printer: " + logmessage);
                        this.MyHost.printers.Add(_newPrinter);
                    }
                
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Printer: ");
            }
        }
        private void getSOUND_CARD()
        {
            try
            {
                ManagementObjectSearcher myAudioObject = new ManagementObjectSearcher("select * from Win32_SoundDevice");
                foreach (ManagementObject obj in myAudioObject.Get())
                {
                    host_sound_card _newSoundCard = new host_sound_card();
                    _newSoundCard.Name = this.GetProperties<string>(obj, "Name").ToLower();
                    _newSoundCard.ProductName = this.GetProperties<string>(obj, "ProductName").ToLower();
                    _newSoundCard.DeviceID = this.GetProperties<string>(obj, "DeviceID").ToLower();
                    _newSoundCard.PowerManagementSupported = this.GetProperties<Boolean>(obj, "PowerManagementSupported");
                    _newSoundCard.Status = this.GetProperties<string>(obj, "Status").ToLower();

                    logger.Info(String.Format("SoundCard: {0} {1}",_newSoundCard.Name,_newSoundCard.Status));

                    this.MyHost.carte_son.Add(_newSoundCard);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, "SoundCard: ");
            }
        }
        private void GetLOGS()
        {
            try
            {
                this.GetLOGS_APP();
                this.GetLOGS_SYS();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Logs:");
            }
        }
        private void GetLOGS_APP()
        {
            try
            {
                EventLog AppEventLog = new EventLog("Application");

                EventLogEntryCollection myEventLogEntryCollection = AppEventLog.Entries;
                AppEventLog.Close();

                int nb_errors = 0;
                int nb_warnings = 0;
                foreach (EventLogEntry MyEventLogEntry in myEventLogEntryCollection)
                {
                    switch (MyEventLogEntry.EntryType)
                    {
                        case EventLogEntryType.Error:
                            {
                                host_log_item FindItem = this.MyHost.logs.app_TopTenErreur.Find(x => x.message == MyEventLogEntry.Message);
                                if (FindItem is null)
                                {
                                    //rien trouvé 
                                    host_log_item _newLogItem = new host_log_item();
                                    _newLogItem.message = MyEventLogEntry.Message;
                                    _newLogItem.count = 1;
                                    this.MyHost.logs.app_TopTenErreur.Add(_newLogItem);
                                }
                                else
                                {
                                    FindItem.count++;
                                }
                                nb_errors++;
                                break;
                            }
                        case EventLogEntryType.Warning:
                            {
                                host_log_item FindItem = this.MyHost.logs.app_TopTenWarrning.Find(x => x.message == MyEventLogEntry.Message);
                                if (FindItem is null)
                                {
                                    //rien trouvé 
                                    host_log_item _newLogItem = new host_log_item();
                                    _newLogItem.message = MyEventLogEntry.Message;
                                    _newLogItem.count = 1;
                                    this.MyHost.logs.app_TopTenWarrning.Add(_newLogItem);
                                }
                                else
                                {
                                    FindItem.count++;
                                }
                                nb_warnings++;
                                break;
                            }
                    }//end switch
                } //end foreach
                this.MyHost.logs.app_nb_erreur = nb_errors;
                this.MyHost.logs.app_nb_warrning = nb_warnings;

                //trie des listes par le nombre d'erreur
                this.MyHost.logs.app_TopTenErreur = this.MyHost.logs.app_TopTenErreur.OrderByDescending(x => x.count).ToList();
                this.MyHost.logs.app_TopTenWarrning = this.MyHost.logs.app_TopTenWarrning.OrderByDescending(x => x.count).ToList();

                //remove les evenements de moins de 30 entrées
                int Index = this.MyHost.logs.app_TopTenErreur.FindIndex(x => x.count < 30);
                int NbItem = this.MyHost.logs.app_TopTenErreur.Count;
                this.MyHost.logs.app_TopTenErreur.RemoveRange(Index, NbItem - Index);

                Index = this.MyHost.logs.app_TopTenWarrning.FindIndex(x => x.count < 30);
                NbItem = this.MyHost.logs.app_TopTenWarrning.Count;
                this.MyHost.logs.app_TopTenWarrning.RemoveRange(Index, NbItem - Index);

                logger.Info(String.Format("LogsApp: #Erreurs={0} #Warrnings={1}", this.MyHost.logs.app_nb_erreur, this.MyHost.logs.app_nb_warrning));

            }
            catch(Exception ex)
            {
                logger.Error(ex, "LogsApp:");
            }
        }
        private void GetLOGS_SYS()
        {
            try { 

            EventLog AppEventLog = new EventLog("System");

            EventLogEntryCollection myEventLogEntryCollection = AppEventLog.Entries;
            AppEventLog.Close();

            int nb_errors = 0;
            int nb_warnings = 0;
            foreach (EventLogEntry MyEventLogEntry in myEventLogEntryCollection)
            {
                switch (MyEventLogEntry.EntryType)
                {
                    case EventLogEntryType.Error:
                        {
                            host_log_item FindItem = this.MyHost.logs.sys_TopTenErreur.Find(x => x.message == MyEventLogEntry.Message);
                            if (FindItem is null)
                            {
                                //rien trouvé 
                                host_log_item _newLogItem = new host_log_item();
                                _newLogItem.message = MyEventLogEntry.Message;
                                _newLogItem.count = 1;
                                this.MyHost.logs.sys_TopTenErreur.Add(_newLogItem);
                            }
                            else
                            {
                                FindItem.count++;
                            }
                            nb_errors++;
                            break;
                        }
                    case EventLogEntryType.Warning:
                        {
                            host_log_item FindItem = this.MyHost.logs.sys_TopTenWarrning.Find(x => x.message == MyEventLogEntry.Message);
                            if (FindItem is null)
                            {
                                //rien trouvé 
                                host_log_item _newLogItem = new host_log_item();
                                _newLogItem.message = MyEventLogEntry.Message;
                                _newLogItem.count = 1;
                                this.MyHost.logs.sys_TopTenWarrning.Add(_newLogItem);
                            }
                            else
                            {
                                FindItem.count++;
                            }
                            nb_warnings++;
                            break;
                        }
                }//end switch
            } //end foreach
            this.MyHost.logs.sys_nb_erreur = nb_errors;
            this.MyHost.logs.sys_nb_warrning = nb_warnings;

            //trie des listes par le nombre d'erreur
            this.MyHost.logs.sys_TopTenErreur = this.MyHost.logs.sys_TopTenErreur.OrderByDescending(x => x.count).ToList();
            this.MyHost.logs.sys_TopTenWarrning = this.MyHost.logs.sys_TopTenWarrning.OrderByDescending(x => x.count).ToList();

            //remove les evenements de moins de 100 entrées
            int Index = this.MyHost.logs.sys_TopTenErreur.FindIndex(x => x.count < 100);
            int NbItem = this.MyHost.logs.sys_TopTenErreur.Count;
            this.MyHost.logs.sys_TopTenErreur.RemoveRange(Index, NbItem - Index);

            Index = this.MyHost.logs.sys_TopTenWarrning.FindIndex(x => x.count < 100);
            NbItem = this.MyHost.logs.sys_TopTenWarrning.Count;
            this.MyHost.logs.sys_TopTenWarrning.RemoveRange(Index, NbItem - Index);

            logger.Info(String.Format("LogsSys: #Erreurs={0} #Warrnings={1}", this.MyHost.logs.sys_nb_erreur, this.MyHost.logs.sys_nb_warrning));

        }
            catch(Exception ex)
            {
                logger.Error(ex, "LogsSys:");
            }
}
        private void getRAM_and_OtherInfo()
        {
            try
            {
                ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_ComputerSystem");

                foreach (ManagementObject Obj in searcher.Get())
                {
                    this.MyHost.ram.TotalPhysicalMemory = this.SizeSuffix(Convert.ToInt64(Obj["TotalPhysicalMemory"]), true);
                    this.MyHost.Model.Marque = Obj["Manufacturer"].ToString();
                    if (this.MyHost.Model.Marque == "VMware, Inc.")
                    {
                        this.MyHost.Model.isVirtualMachine = true;
                    }
                    this.MyHost.Model.Model = Obj["Model"].ToString();
                }

                searcher = new ManagementObjectSearcher("root\\CIMV2",
                        "SELECT * FROM Win32_ComputerSystemProduct");

                foreach (ManagementObject Obj in searcher.Get())
                {
                    this.MyHost.Model.ServiceTag = Obj["IdentifyingNumber"].ToString();
                    this.MyHost.Model.SerialNumber = Obj["UUID"].ToString();

                }

                logger.Info(String.Format("Ram: {0}", this.MyHost.ram.TotalPhysicalMemory));

                if(this.MyHost.Model.Marque.ToLower().Contains("dell"))
                {
                    logger.Info(String.Format("Model: {0}", this.MyHost.Model.Marque));
                    logger.Info(String.Format("Model: {0}", this.MyHost.Model.Model));
                    logger.Info(String.Format("Model: {0}", this.MyHost.Model.ServiceTag));
                }
                else
                {
                    logger.Info(String.Format("Model: {0}", this.MyHost.Model.Marque));
                    logger.Info(String.Format("Model: {0}", this.MyHost.Model.Model));
                }

            }
            catch(Exception ex)
            {
                logger.Error(ex, "Ram:");
            }

        }

            

        private List<string> GetFolderProfiles()
        {
          //List<string> _ret = new List<string>();

            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            userProfilePath = userProfilePath.Substring(0, userProfilePath.Length - this.MyHost.users.CurentLogUserName.Length);
            List<string> _ret = Directory.GetDirectories(userProfilePath).ToList<string>();
            if (_ret.Contains("C:\\Users\\All Users"))
            {
                _ret.Remove("C:\\Users\\All Users");
            }
            if (_ret.Contains("C:\\Users\\Default User"))
            {
                _ret.Remove("C:\\Users\\Default User");
            }
            if (_ret.Contains("C:\\Users\\defaultuser0"))
            {
                _ret.Remove("C:\\Users\\defaultuser0");
            }
            if (_ret.Contains("C:\\Users\\Default"))
            {
                _ret.Remove("C:\\Users\\Default");
            }
            if (_ret.Contains("C:\\Users\\Public"))
            {
                _ret.Remove("C:\\Users\\Public");
            }
            if (_ret.Contains("C:\\Users\\" + this.MyHost.users.CurentLogUserName))
            {
                _ret.Remove("C:\\Users\\" + this.MyHost.users.CurentLogUserName);
            }
            List<string> CleanProfiles = new List<string>();
            foreach (string profile in _ret)
            {
                CleanProfiles.Add(profile.Split('\\')[2]);
            }

            _ret = CleanProfiles;
            return _ret;
        }


        private void GetHostUser()
        {
            try
            {
                //bool admin = this.IsAdministrator();
                //this.GetGroupsForCurentDomainUser();
                //this.GetLocalUser();
                //this.GetLocalGroup();

                this.MyHost.users.CurentLogUserName = Environment.UserName;
                this.MyHost.users.IsCurentLogUserIsAdmin = this.IsAdministrator();
                this.MyHost.users.profiles = this.GetFolderProfiles();
                this.MyHost.users.localGroups = this.GetLocalGroup();
                this.MyHost.users.localUsers = this.GetLocalUser();

                logger.Info(string.Format("User: login={0} #other={1}", this.MyHost.users.CurentLogUserName, this.MyHost.users.profiles.Count.ToString()));
                                
                //EventLog logs = new EventLog("Security");
                //List<string> usr = new List<string>();
                //foreach (EventLogEntry entry in logs.Entries)
                //{
                    
                //    if(entry.InstanceId == 4624)
                //    {
                //        string usrLogin = entry.ReplacementStrings[5];
                        
                //        if(usrLogin == this.MyHost.users.CurentLogUserName)
                //        {
                //            int fff = 1;
                //            DateTime sss = entry.TimeGenerated;
                //        }

                //    }
                 
                //}
            }
            catch(Exception ex)
            {
                logger.Error(ex, "User:");
            }


       


        }

      
        #endregion

        #region constructeur
        public LibWinInventory()
        {
            this.MyHost = new host();
            this.MyHost.name = System.Environment.MachineName.ToLower();

        }
        #endregion

        #region public_method
        public string ExportToJsonString(host dataItem)
        {
            string json = JsonConvert.SerializeObject(dataItem, Formatting.Indented);
            return json;
        }
        public host RunDetection()
        {
            logger.Info("start RunDetection");

            //List<string> ExeFile = this.GetFileList("*.exe", "c:\\").ToList<string>();
            this.getPRINTER();
            
                this.GetSoftware();
                this.getOS();
                this.getCPU();
                this.getDISK();
                this.getVIDEO();
                this.getNETWORK();
                this.getSOUND_CARD();
                this.getRAM_and_OtherInfo();
                this.GetLOGS();
                this.GetMonitor();
                this.GetHostUser();
                logger.Info("end_RunDetection");
                return this.MyHost;            
        }
        #endregion
    }
}