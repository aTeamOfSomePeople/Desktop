using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для Proxy.xaml
    /// </summary>
    public partial class Proxy : Window
    {
        public Proxy()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Net.WebRequest.DefaultWebProxy = null;
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Net.WebRequest.DefaultWebProxy = new WebProxy()
                {
                    Address = new Uri(textBox.Text),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(textBox1.Text, passwordBox.Password)
                };
                Close();
            }
            catch
            {

            }
        }
    }
}
