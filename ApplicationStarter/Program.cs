using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ApplicationStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var processes = new List<Process>();
            var exeNames = new string[]
            {
                "ServerSide.exe",
                "ClientSide.exe",
                "Data.exe"
            };

            foreach (var exeName in exeNames)
            {
                try
                {
                    ProcessStartInfo processStarter = new ProcessStartInfo();

                    processStarter.FileName = exeName;
                    processStarter.UseShellExecute = true;
                    processes.Add(Process.Start(processStarter));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadKey();
            
            processes.ForEach(p => p.Close());
        }
    }
}