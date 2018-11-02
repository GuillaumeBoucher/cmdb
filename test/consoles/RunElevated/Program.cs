using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunElevated
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
            }
            else
            {
                if (args.Length == 1)
                {
                    string exe = args[0];
                    if (File.Exists(exe))
                    {
                        ExecuteAsAdmin(exe);
                    }
                    else
                    {
                        Console.WriteLine("Commande {0} not found", exe);
                    }

                }
                else
                {
                    PrintHelp();
                }
            }

        }

        public static void PrintHelp()
        {
            Console.Clear();
            Console.WriteLine("RunElevated <executable>");

        }
        public static void ExecuteAsAdmin(string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
        }
    }
}
