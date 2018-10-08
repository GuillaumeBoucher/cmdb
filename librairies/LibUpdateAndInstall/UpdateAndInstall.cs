using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace LibUpdateAndInstall
{
    
    public class UpdateAndInstall
    {
        private string AppName { get; set; }
        private string DownloadUrl { get; set; }

        //constructeur
        public UpdateAndInstall(string AppName,string DownloadUrl)
        {
            this.AppName = AppName;
            this.DownloadUrl = DownloadUrl;
        }

        public string GetServerCurentVersion()
        {
            string _ret = "";
            string url = this.DownloadUrl + "version.json";
            HttpWebRequest httpWebRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;

            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                {
                    //throw new Exception(string.Format("Server error (HTTP {0}: {1}).",
                    //httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    _ret = "404";
                }

                Stream stream = httpWebResponse.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string JsonData = sr.ReadToEnd();
                Models.version.version ver = JsonConvert.DeserializeObject<Models.version.version>(JsonData);

                if (ver == null)
                {
                    //log ?
                }
                else
                {
                    
                    _ret = ver.ServerVersion;
                }
            }


            return _ret;
        }

      

        private void DownloadZip(string version)
        {
            string remoteUri = this.DownloadUrl + version + ".zip";
            string filename = "c:\\DSI\\temp\\"+version+".zip";
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myWebClient.DownloadFile(remoteUri, filename);           
        }

        private void UnzipFile(string version)
        {
            string filename = "c:\\DSI\\temp\\" + version + ".zip";
            if (File.Exists(filename))
            {
                using (Stream stream = File.OpenRead(filename))
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            //Console.WriteLine(reader.Entry.Key);
                            reader.WriteEntryToDirectory(@"C:\DSI\" + this.AppName + "\\" + version + "\\", new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
                File.Delete(filename);
            }
        }

        public void Update(string version)
        {
            this.DeleteCronTask();

            this.DownloadZip(version);
            this.UnzipFile(version);
            this.CreateCronTask(version);
        }
        private void DeleteCronTask()
        {
            string CronName = this.AppName + "_";
            List<string> AllTasks = this.AllCronTask(CronName);

            using (TaskService ts = new TaskService())
            {
                foreach(string taskName in AllTasks)
                {
                    ts.RootFolder.DeleteTask(taskName);
                }
            }
        }

        private List<string> AllCronTask(string CronName)
        {
            List<string> _ret = new List<string>();
            using (TaskService ts = new TaskService())
            {
                TaskCollection tasks = ts.RootFolder.GetTasks();
                foreach (Microsoft.Win32.TaskScheduler.Task t in tasks)
                {
                    if (t.Name.StartsWith(CronName))
                    {
                        _ret.Add(t.Name);
                    }
                }
            }
            return _ret;
        }

        private Boolean CronTaskExist(string version)
        {
            Boolean _ret = false;
            string CronName = this.AppName + "_" + version;
            
            using (TaskService ts = new TaskService())
            {
                TaskCollection tasks = ts.RootFolder.GetTasks();
                foreach (Microsoft.Win32.TaskScheduler.Task t in tasks)
                {
                    if (t.Name == CronName)
                    {
                        _ret = true;
                    }

                }
            }
            return _ret;
        }

        public void InstallAndCroned(string version)
        {
            //creation du repertoire 
            if (!Directory.Exists("C:\\DSI"))
            {
                Directory.CreateDirectory("C:\\DSI");
            }
            if (!Directory.Exists("C:\\DSI\\temp"))
            {
                Directory.CreateDirectory("C:\\DSI\\temp");
            }

            this.DeleteCronTask();

            this.DownloadZip(version);

            this.UnzipFile(version);

            this.CreateCronTask(version);

            //add user right


            //donner les droits a l'utilisateurs local


        }

            public Boolean IsInstallAndCroned(string version)
        {
            Boolean _ret = false;
            Boolean FolderExist = false;
            Boolean TaskExist = false;

            //Test if file Exist
            if (Directory.Exists("c:\\DSI\\"+this.AppName+"\\"+ version + "\\"))
            {
                FolderExist = true;
            }
            else
            {
                FolderExist = false;
            }

            //Test if file Cron Exit
            TaskExist = this.CronTaskExist(version);

            if(TaskExist && FolderExist)
            {
                _ret = true;
            }
            return _ret;
        }

        private void CreateCronTask(string version)
        {
            string CronName = this.AppName + "_" + version;

            // Create a new task definition for the local machine and assign properties
            TaskDefinition td = TaskService.Instance.NewTask();
            td.RegistrationInfo.Description = this.AppName + "_" + version;

            // Add a trigger that, starting tomorrow, will fire every other week on Monday
            // and Saturday and repeat every 10 minutes for the following 11 hours
            //MonthlyTrigger 
            WeeklyTrigger wt = new WeeklyTrigger();
            DateTime now = DateTime.Today;
            DateTime startDate = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
            DateTime endDate = new DateTime(now.Year, now.Month, now.Day, 14, 0, 0);
            TimeSpan timeSpan = endDate - startDate;
            var randomTest = new Random();
            TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
            DateTime newDate = startDate + newSpan;

            wt.StartBoundary = newDate;
            wt.DaysOfWeek = DaysOfTheWeek.Friday | DaysOfTheWeek.Tuesday;
            //wt.WeeksInterval = 2;
            //wt.Repetition.Duration = TimeSpan.FromHours(11);
            //wt.Repetition.Interval = TimeSpan.FromMinutes(10);
            td.Triggers.Add(wt);

            // Create an action that will launch Notepad whenever the trigger fires
            td.Actions.Add("C:\\DSI\\"+ this.AppName +"\\"+ version +"\\"+this.AppName+".exe", "croned", "C:\\DSI\\WinInventory\\" + version + "\\");

            // Register the task in the root folder of the local machine
            TaskService.Instance.RootFolder.RegisterTaskDefinition(CronName, td);
        }

      
  

      
    }
}
