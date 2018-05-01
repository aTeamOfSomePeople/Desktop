using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Registration signin = new Registration();
            this.Close();
            signin.Show();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                API.Accounts.Account account = await API.Accounts.Auth(login.Text, password.Password);
                if (account != null)
                {
                    API.Users.User user = await API.Users.GetUserInfo(account.userId);
                    Window main = new Main(account, user);
                    this.Close();
                    main.Show();
                }
                else
                {
                    textBlock.Text = "Введёные данные не верны";
                }
            }
            catch(System.Net.Http.HttpRequestException)
            {
                textBlock.Text = "Отсутствует подключение";
            }
            
            

        }

        private async void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VkAutho wind = new VkAutho();
            wind.ShowDialog();
            string url;
            url = wind.VkToken.Source.AbsoluteUri;
            char[] separators = { '=', '&' };
            string[] responseContent = url.Split(separators);
            string accessToken = responseContent[1];
            API.Accounts.Account account = await API.Accounts.OAuth(accessToken, API.Service.Vk);
            if (account != null)
            {
                API.Users.User user = await API.Users.GetUserInfo(account.userId);
                Window main = new Main(account, user);
                this.Close();
                main.Show();
            }
        }

        private async void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GoogleAuth wind = new GoogleAuth();
            wind.ShowDialog();
            string url;
            url = wind.Google.Source.AbsoluteUri;
            char[] separators = { '=', '#' };
            string[] responseContent = url.Split(separators);
            string code = responseContent[1];
            string client_id = "192203327694-e38jj58iefos1bi6af3luv62tk969e23.apps.googleusercontent.com";
            string client_secret = "gwmIbi3e4xOdDdOCddv6EiTk";
            string redirect_url = "https://www.google.com/blank.html";

            GooglePlusAccessToken googleToken = new GooglePlusAccessToken();
            googleToken = googleToken.GetToken(code, client_id, client_secret, redirect_url);

            API.Accounts.Account account = await API.Accounts.OAuth(googleToken.access_token, API.Service.Google);
            if (account != null)
            {
                API.Users.User user = await API.Users.GetUserInfo(account.userId);
                Window main = new Main(account, user);
                this.Close();
                main.Show();
            }

        }

        private async void image2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InstagramAuth wind = new InstagramAuth();
            wind.ShowDialog();
            string url = wind.Instagram.Source.AbsoluteUri;
            char[] separators = { '=', '.' };
            string[] responseContent = url.Split(separators);
            string accessToken = responseContent[5] + "." + responseContent[6];

            API.Accounts.Account account = await API.Accounts.OAuth(accessToken, API.Service.Instagram);
            if (account != null)
            {
                API.Users.User user = await API.Users.GetUserInfo(account.userId);
                Window main = new Main(account, user);
                this.Close();
                main.Show();
            }
        }


        public class GooglePlusAccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string id_token { get; set; }
            public string refresh_token { get; set; }

            public GooglePlusAccessToken GetToken(string code, string client_id, string client_secret, string redirect_url)
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/oauth2/v4/token");
                webRequest.Method = "POST";
                string Parameters = "code=" + code + "&client_id=" + client_id + "&client_secret=" + client_secret + "&redirect_uri=" + redirect_url + "&grant_type=authorization_code";
                byte[] byteArray = Encoding.UTF8.GetBytes(Parameters);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;
                Stream postStream = webRequest.GetRequestStream();
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                WebResponse response = webRequest.GetResponse();
                postStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(postStream);
                string responseFromServer = reader.ReadToEnd();

                GooglePlusAccessToken serStatus = JsonConvert.DeserializeObject<GooglePlusAccessToken>(responseFromServer);
                return serStatus;
            }
        }
    }
}
