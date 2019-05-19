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
    /// Логика взаимодействия для ExceptionPage.xaml
    /// </summary>
    public partial class ExceptionPage : Page
    {
        public ExceptionPage()
        {
            InitializeComponent();
        }

        private void DeleteException_Click(object sender, RoutedEventArgs e)
        {
            DeleteExceptionEvent?.Invoke(this, new AntivirusLibrary.Events.AddFileInExceptionEventArgs(((Button)sender).DataContext.ToString()));
        }

        public event EventHandler<AntivirusLibrary.Events.AddFileInExceptionEventArgs> DeleteExceptionEvent;
    }
}
