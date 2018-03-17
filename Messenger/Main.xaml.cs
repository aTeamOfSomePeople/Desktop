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
        }
        
        private void createChat_Click(object sender, RoutedEventArgs e)
        {
            var users = new HashSet<ZeroAPI.User>();
            foreach (var us in listBox3.Items)
            {
                users.Add(ZeroAPI.User.GetInfo(int.Parse(us.ToString())));
            }
            user.CreateChat(textBox.Text, users);
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
                listBox3.Items.Add(users[listBox2.SelectedIndex].Id);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            searchUsers.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }
        ~Main()
        {
            ZeroAPI.Connection.Disconnect();
        }
    }
}
