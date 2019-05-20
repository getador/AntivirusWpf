using System;
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
    /// Логика взаимодействия для VirusPage.xaml
    /// </summary>
    public partial class VirusPage : Page
    {
        public VirusPage()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteFileEvent?.Invoke(this, new AntivirusLibrary.Events.AddFileInExceptionEventArgs(((Button)sender).DataContext.ToString()));
        }

        private void AddException_Click(object sender, RoutedEventArgs e)
        {
            AddFileInExceptionEvent?.Invoke(this, new AntivirusLibrary.Events.AddFileInExceptionEventArgs(((Button)sender).DataContext.ToString()));
           // Content = "{Binding AddInExceptionButtonContext}"
        }
        public event EventHandler<AntivirusLibrary.Events.AddFileInExceptionEventArgs> AddFileInExceptionEvent;
        public event EventHandler<AntivirusLibrary.Events.AddFileInExceptionEventArgs> DeleteFileEvent;
    }
}
