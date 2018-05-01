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
    /// Логика взаимодействия для GoogleAuth.xaml
    /// </summary>
    public partial class GoogleAuth : Window
    {
        public GoogleAuth()
        {
            InitializeComponent();
            Google.Source = new Uri("http://accounts.google.com/o/oauth2/auth?scope=https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/userinfo.email&access_type=offline&prompt=consent&redirect_uri=https://www.google.com/blank.html&response_type=code&client_id=192203327694-e38jj58iefos1bi6af3luv62tk969e23.apps.googleusercontent.com");
        }

        private void Google_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Google.Source.LocalPath.ToString() == "/blank.html")
            {
                Close();
            }
        }
    }
}
