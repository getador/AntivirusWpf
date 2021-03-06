﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Antivirus.Pages
{
    /// <summary>
    /// Логика взаимодействия для DangerProccesPage.xaml
    /// </summary>
    public partial class DangerProcessPage : Page
    {
        public DangerProcessPage()
        {
            InitializeComponent();
        }

        private void AddInException_Click(object sender, RoutedEventArgs e)
        {
            AddFileInExceptionEvent?.Invoke(this, new AntivirusLibrary.Events.AddFileInExceptionEventArgs(((AntivirusLibrary.ProcessDange)((Button)sender).DataContext).Path));
        }

        private void StopProcess_Click(object sender, RoutedEventArgs e)
        {
            KillProcessEvent?.Invoke(this, new AntivirusLibrary.Events.AddFileInExceptionEventArgs(((AntivirusLibrary.ProcessDange)((Button)sender).DataContext).Process.ProcessName));
        }

        public event EventHandler<AntivirusLibrary.Events.AddFileInExceptionEventArgs> AddFileInExceptionEvent;
        public event EventHandler<AntivirusLibrary.Events.AddFileInExceptionEventArgs> KillProcessEvent;
    }
}
