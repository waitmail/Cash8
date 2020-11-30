using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Cash8
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (IsApplicationRunningOnMono(Application.ProductName + ".exe"))
            {
                MyMessageBox mmb = new MyMessageBox("Это второй экземпляр программы, разрешается запуск только одного экземпляра", "Проверка дублей запущенной программы");
                mmb.ShowDialog();
                return;
            }
            Application.Run(new Main());
        }

        private static bool IsApplicationRunningOnMono(string processName)
        {
            var processFound = 0;

            Process[] monoProcesses;
            ProcessModuleCollection processModuleCollection;

            //find all processes called 'mono', that's necessary because our app runs under the mono process! 
            monoProcesses = Process.GetProcessesByName("mono");

            for (var i = 0; i <= monoProcesses.GetUpperBound(0); ++i)
            {
                processModuleCollection = monoProcesses[i].Modules;

                for (var j = 0; j < processModuleCollection.Count; ++j)
                {
                    if (processModuleCollection[j].FileName.EndsWith(processName))
                    {
                        processFound++;
                    }
                }
            }

            //we don't find the current process, but if there is already another one running, return true! 
            return (processFound == 1);
        } 
    }
}
