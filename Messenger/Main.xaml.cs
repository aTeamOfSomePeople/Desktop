using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using Microsoft.Win32;

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private ZeroAPI.User user { get; set; }
        private ZeroAPI.Chat currentChat { get; set; }
        private List<ZeroAPI.Chat> chats = new List<ZeroAPI.Chat>();
        private List<ZeroAPI.User> users = new List<ZeroAPI.User>();
        public int lol = 1;

        public Main(ZeroAPI.User user)
        {
            InitializeComponent();
            this.user = user;
            currentChat = null;
            textBlock.Text = user.Name;
            ZeroAPI.Connection.Connect(user, e => Dispatcher.Invoke(() =>
            {
                if (currentChat != null && currentChat.Id == e.ChatId)
                {
                    listBox1.Items.Add(ZeroAPI.User.GetInfo(e.UserId).Name + " > " + e.Text + " | " + e.Date.ToShortTimeString());
                }
            }), e => Dispatcher.Invoke(() => { chats.Add(e); listBox.Items.Add(e.Name); }));

            var img = new BitmapImage();
            img.BeginInit();
            try
            {
                img.StreamSource = new System.IO.MemoryStream(ZeroAPI.Utils.GetFileAsBytes(user.Avatar));
                img.EndInit();
                image1.Source = img;
            }
            catch { }
        }
        
        private void createChat_Click(object sender, RoutedEventArgs e)
        {
            var users = new HashSet<ZeroAPI.User>();
            foreach (var us in listBox3.Items)
            {
                users.Add(ZeroAPI.User.GetInfo(int.Parse(us.ToString())));
            }
            user.CreateChat(textBox.Text, ZeroAPI.ChatType.Public, users);
            searchUsers.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        private void listBox_Loaded(object sender, RoutedEventArgs e)
        {
            chats.Clear();
            foreach (var chat in user.GetChats())
            {
                chats.Add(chat);
                listBox.Items.Add(chat.Name);
            }
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                listBox1.Items.Clear();
                foreach (var message in (user.GetMessages(chats[listBox.SelectedIndex])))
                {
                    listBox1.Items.Add(ZeroAPI.User.GetInfo(message.UserId).Name + " > " + message.Text + " | " + message.Date.ToShortTimeString());
                }
                currentChat = chats[listBox.SelectedIndex];
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            user.SendMessage(textBox2.Text, currentChat);
            textBox2.Clear();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            listBox2.Items.Clear();
            users.Clear();
            foreach (var user in ZeroAPI.User.FindUsers(textBox3.Text))
            {
                users.Add(user);
                listBox2.Items.Add(user.Name);
            }

        }

        private void listBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                UserInformations.Visibility = Visibility.Visible;
                searchUsers.Visibility = Visibility.Hidden;
                NameOfSearchUser.Text = users[listBox2.SelectedIndex].Name;

                var img = new BitmapImage();
                img.BeginInit();
                try
                {
                    img.StreamSource = new System.IO.MemoryStream(ZeroAPI.Utils.GetFileAsBytes(users[listBox2.SelectedIndex].Avatar));
                    img.EndInit();
                    SearchUserAvatar.Source = img;
                }
                catch { }
                


                //    listBox3.Items.Add(users[listBox2.SelectedIndex].Id);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            searchUsers.Visibility = Visibility.Visible;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 20;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = false;
        }


        ~Main()
        {
            ZeroAPI.Connection.Disconnect();
        }

        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            textBox.Clear();
            textBox3.Clear();
            searchUsers.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 20;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = false;
            nameSetting.Text = user.Name;
            loginSetting.Text = "Денис запретил";
            var img = new BitmapImage();
            img.BeginInit();
            try
            {
                img.StreamSource = new System.IO.MemoryStream(ZeroAPI.Utils.GetFileAsBytes(user.Avatar));
                img.EndInit();
                avatarSetting.Source = img;
            }
            catch { }
            bioSetting.Text = "Появится в новой версии";
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            UserInformations.Visibility = Visibility.Hidden;
            searchUsers.Visibility = Visibility.Visible;
        }

        private void CreateChatOne_Click(object sender, RoutedEventArgs e)
        {
            var usersChat = new HashSet<ZeroAPI.User>();
            usersChat.Add(ZeroAPI.User.GetInfo(int.Parse(users[listBox2.SelectedIndex].Id.ToString())));
            user.CreateChat(users[listBox2.SelectedIndex].Name, ZeroAPI.ChatType.Public, usersChat);
            searchUsers.Visibility = Visibility.Hidden;
            UserInformations.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        private void buttonSetting_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            AllSettings.Visibility = Visibility.Visible;
        }

        private void avatarSetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "jpg|*.jpg| bmp|*.bmp";

            Nullable<bool> result = dlg.ShowDialog();

            string filename = "";

            if (result == true)
            {
                filename = dlg.FileName;
            }
            user.ChangeAvatar(filename);
            user = ZeroAPI.User.GetInfo(user.Id);

            var img = new BitmapImage();
            img.BeginInit();
            try
            {
                img.StreamSource = new System.IO.MemoryStream(ZeroAPI.Utils.GetFileAsBytes(user.Avatar));
                img.EndInit();
                image1.Source = img;
                
            }
            catch { }
        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            AllSettings.Visibility = Visibility.Hidden;
        }

        private void SaveChange_Click(object sender, RoutedEventArgs e)
        {
            if(NameChange.Text != "")
            {
                user.ChangeName(NameChange.Text);
                user = ZeroAPI.User.GetInfo(user.Id);
                textBlock.Text = user.Name;
                nameSetting.Text = user.Name;
            }
            if(PasswordChange.Text != "")
            {
                user.ChangePassword(PasswordChange.Text);
            }
            Settings.Visibility = Visibility.Visible;
            AllSettings.Visibility = Visibility.Hidden;
        }
    }
}
