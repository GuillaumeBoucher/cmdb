using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace GetComputerNameAndLocalUser
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Environment.MachineName;
            string user = Environment.UserName;

            Console.Clear();
            Console.WriteLine("-------------------------------------------------");
            string adminOrNot = "";
            if (IsAdministrator() == false)
            {
                adminOrNot = "NON";
            }
            else
            {
                adminOrNot = "OUI";
            }
            Console.WriteLine("Machine :{0}   User :{1}  Admin: {2}",host,user,adminOrNot);
            Console.WriteLine("");
            try
            {
                Directory.CreateDirectory("c:\\test");
                Console.WriteLine("Create c:\\text ok");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception source: {0} mesg:{1}", ex.Source, ex.Message);
            }
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Appuyer sur une touche pour quitter");
            Console.ReadKey();

            //if (IsAdministrator() == false)
            //{
            //    // Restart program and run as admin
            //    var exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
            //    startInfo.Verb = "runas";
            //    System.Diagnostics.Process.Start(startInfo);
            //    Application.Current.Shutdown();
            //    return;
            //}
        }


       

    private static bool IsAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
}
