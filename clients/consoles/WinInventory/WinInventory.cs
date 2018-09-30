using lib.WinInventory;
using model.hosts;
using NLog;
using System;
using System.Configuration;
using System.IO;
using WinInventory;


namespace WinInventory
{
    class WinInventory
    {
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
                //Read config and define export path
                Init();
                //ScanHost
                ScanHostConfig();
                logger.Info(String.Format("JsonExport: {0} Size={1}", FileName, JsonHostConfigSize ));
                LocalSave();
                MoveToServeur();
                logger.Info("END");
                logger.Info("-------------------------------------");



            }
            catch (Exception ex)
            {
                logger.Error(ex, "FindHostInfo");
            }
        }
        private static void Init()
        {  
            ExportPath = ConfigurationManager.AppSettings["ExportPath"].ToString();
            ExportServer = ConfigurationManager.AppSettings["ExportServer"].ToString();
            ExportShare = ConfigurationManager.AppSettings["ExportShare"].ToString();

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
