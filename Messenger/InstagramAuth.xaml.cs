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

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для InstagramAuth.xaml
    /// </summary>
    public partial class InstagramAuth : Window
    {
        public InstagramAuth()
        {
            InitializeComponent();
            Instagram.Source = new Uri("https://instagram.com/oauth/authorize/?client_id=7e7352621ade4e95b7211ff823ab7d30&redirect_uri=https://google.com/blank.html&response_type=token");
        }

        private void Instagram_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Instagram.Source.LocalPath.ToString() == "/blank.html")
            {
                Close();
            }
        }
    }
}
