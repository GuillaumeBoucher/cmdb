using LibUpdateAndInstall;
using lib.WinInventory;
using model.hosts;
using NLog;
using System;
using System.Configuration;
using System.IO;
using XperiCode.Impersonator;

namespace WinInventory
{
    class WinInventory
    {
        private static Boolean IsLauncheFromCron { get; set; }
        private static string UpdateUrl { get; set; }
        private static string LocalVersion { get; set; }
        private static string ExportPath { get; set; }
        private static string ExportServer { get; set; }
        private static string ExportShare { get; set; }
        private static string JsonHostConfig  { get; set; }
        private static string JsonHostConfigSize { get; set; }
        private static string BasePath { get; set; }
        private static string FileName { get; set; }
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                logger.Info("START");
                logger.Info("-------------------------------------");

                using (new Impersonator(@"csa\adm_boucher", "lnaf82gaz!"))
                {
                    if (args.Length > 0)
                    {
                        IsLauncheFromCron = true;
                        logger.Info("LauncheFromCron: True");
                    }
                    else
                    {
                        IsLauncheFromCron = false;
                        logger.Info("LauncheFromCron: False");
                    }                    
                    Init(); //Read config and define export path
                    logger.Info("Initialisation done");
                    if (IsInstallAndCroned())
                    {
                        CheckUpdate();
                        logger.Info("Update");
                    }
                    else
                    {
                        InstallAndCroned();
                        logger.Info("Install");
                    }
                }
                if(IsLauncheFromCron)
                {
                    ScanHostConfig();
                    logger.Info(String.Format("JsonExport: {0} Size={1}", FileName, JsonHostConfigSize));
                    LocalSave();
                    MoveToServeur();
                }
                logger.Info("END");
                logger.Info("-------------------------------------");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "FindHostInfo");
            }
        }

        private static void InstallAndCroned()
        {
            UpdateAndInstall _obj = new UpdateAndInstall("WinInventory", UpdateUrl);
            _obj.InstallAndCroned(LocalVersion);
        }

        private static void CheckUpdate()
        {
            UpdateAndInstall _obj = new UpdateAndInstall("WinInventory", UpdateUrl);

            string ServeurCurentVersion = _obj.GetServerCurentVersion();

            if(ServeurCurentVersion != LocalVersion)
            {
                _obj.Update(ServeurCurentVersion);
            }
        }
        private static Boolean IsInstallAndCroned()
        {
            Boolean _ret = false;
            UpdateAndInstall _obj = new UpdateAndInstall("WinInventory", UpdateUrl);
            _ret = _obj.IsInstallAndCroned(LocalVersion);

            return _ret;
        }

        private static void Init()
        {  
            ExportPath = ConfigurationManager.AppSettings["ExportPath"].ToString();
            ExportServer = ConfigurationManager.AppSettings["ExportServer"].ToString();
            ExportShare = ConfigurationManager.AppSettings["ExportShare"].ToString();
            LocalVersion = ConfigurationManager.AppSettings["LocalVersion"].ToString();
            UpdateUrl = ConfigurationManager.AppSettings["UpdateUrl"].ToString();

            BasePath = AppDomain.CurrentDomain.BaseDirectory;
            FileName = string.Format("{0}.{1:yyyy_MM_dd_hh_mm_ss_tt}.json", Environment.MachineName, DateTime.Now);

             if (!Directory.Exists(BasePath+"\\"+ ExportPath))
             {
                 Directory.CreateDirectory(BasePath + "\\" + ExportPath);
             }
        }
        private static void ScanHostConfig()
        {
            LibWinInventory _obj = new LibWinInventory();            
            host DetectedHost   = _obj.RunDetection();
            JsonHostConfig = _obj.ExportToJsonString(DetectedHost);
            JsonHostConfigSize = JsonHostConfig.Length.ToString();
        }
        private static void LocalSave()
        {
            StreamWriter sr = File.CreateText(BasePath + "\\" + ExportPath + "\\" + FileName);
            sr.Write(JsonHostConfig);
            sr.Close();
            sr.Dispose();
        }
        private static void MoveToServeur()
        {
            //Check If NetWork is 
            string source_Files = BasePath + ExportPath + "\\*.json";
            string dest_unc_folder = "\\\\" + ExportServer + "\\" + ExportShare + "\\";
            if(Directory.Exists(dest_unc_folder))
            {

                DirectoryInfo d = new DirectoryInfo(BasePath + ExportPath); 
                FileInfo[] Files = d.GetFiles("*.json");                 
                foreach (FileInfo file in Files)
                {
                    File.Move(file.FullName, dest_unc_folder + "\\" + file.Name);
                }
            }
            else
            {
                logger.Error("Destination folder not accessible");
            }



            //     File.Copy(LocalfileName, destFi;le);
            //     logger.Info("END");
            //     logger.Info("-------------------------------------");
            // }

        }
    }
}
