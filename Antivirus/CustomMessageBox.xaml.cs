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
using System.Windows.Shapes;

namespace Antivirus
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox()
        {
            InitializeComponent();
        }
        int status = 0;
        public int ShopMessage(string title,string message,string firstButtonText,string secondButtonText)
        {
            Title = title;
            MessagePlace.Text = message;
            FirstButton.Content = firstButtonText;
            SecondButton.Content = secondButtonText;
            while ((bool)ShowDialog())
            {

            }
            return status;
        }

        private void FirstButton_Click(object sender, RoutedEventArgs e)
        {
            status = 1;
            Close();
        }

        private void SecondButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
