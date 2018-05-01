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
using System.Windows.Navigation;
using System.Reflection;

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для VkAutho.xaml
    /// </summary>
    public partial class VkAutho : Window
    {
        public VkAutho()
        {
            InitializeComponent();
            VkToken.Source = new Uri("https://oauth.vk.com/authorize?client_id=6412732&display=page&scope=offline,email&redirect_uri=http://oauth.vk.com/blank.html&response_type=token");
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            dynamic activeX = VkToken.GetType().InvokeMember("ActiveXInstance",
                   BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                   null, VkToken, new object[] { });

            activeX.Silent = true;
        }

        private void VkToken_Navigated(object sender, NavigationEventArgs e)
        {
            if (VkToken.Source.LocalPath.ToString() == "/blank.html")
            {
                Close();
            }

        }
    }
}
