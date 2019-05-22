using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Antivirus
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!InstanceCheck())
            {
                Environment.Exit(0);
            }
        }
        static Mutex InstanceCheckMutex;
        private bool InstanceCheck()
        {
            bool isNew;
            InstanceCheckMutex = new Mutex(true, "Antivirus application", out isNew);
            return isNew;
        }
    }
}
