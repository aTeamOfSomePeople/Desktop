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
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await API.Accounts.Register(login.Text, password.Password, name.Text);
                //ZeroAPI.User.Register(name.Text, login.Text, password.Text);
                API.Accounts account = await API.Accounts.Auth(login.Text, password.Password);
                //var user = ZeroAPI.User.Authorization(login.Text, password.Text);
                if (account != null)
                {
                    API.Users user = await API.Users.GetUserInfo(account.userId);
                    Window main = new Main(account, user);
                    this.Close();
                    main.Show();
                }
                else
                {
                    textBlock.Text = "Такой пользователь уже существует";
                }
            }
            catch(System.Net.Http.HttpRequestException)
            {
                textBlock.Text = "Отсутствует подключение";
            }
            
        }
    }
}
