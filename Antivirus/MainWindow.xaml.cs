using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Antivirus
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Window window;
        public MainWindow()
        {
            InitializeComponent();
            window = this;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private static readonly Regex regex = new Regex("[^0-9]+");
        private void IsTextAllowed(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (((TextBox)sender).Text == string.Empty || (Convert.ToInt32(((TextBox)sender).Text+e.Text)  > 0 && Convert.ToInt32(((TextBox)sender).Text + e.Text) <= Environment.ProcessorCount))
                {
                    e.Handled = regex.IsMatch(e.Text);
                }
                else
                {
                    e.Handled = true;
                }
                // && !(Convert.ToInt32(((TextBox)sender).Text) > 0 && Convert.ToInt32(((TextBox)sender).Text) <= Environment.ProcessorCount);
            }
            catch (Exception)
            {
                e.Handled = true;
                //((TextBox)sender).Text.Remove(1, ((TextBox)sender).Text.Length);
            }
        }
    }
}
