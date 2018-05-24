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
using System.Collections.ObjectModel;

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        private API.Users user { get; set; }
        private API.Accounts account { get; set; }
        private API.Chats currentChat { get; set; }
        //private List<ZeroAPI.Chat> chats = new List<ZeroAPI.Chat>();
        private List<API.Chats> chats = new List<API.Chats>();
        private List<API.Users> users = new List<API.Users>();
        private ObservableCollection<API.Messages> messages = new ObservableCollection<API.Messages>();
        private List<long> pictureIds = new List<long>();

        public Main(API.Accounts account, API.Users user)
        {
            InitializeComponent();
            currentChat = null;
            this.user = user;
            this.account = account;
            textBlock.Text = user.name;
            ListViewMes.ItemsSource = messages;
            messages.CollectionChanged += Messages_CollectionChanged;
            API.Connection.Connect(account.accessToken, e => Dispatcher.Invoke(async () =>
            {
                var message = await API.Messages.GetMessage(account.accessToken, e);
                if (currentChat != null && currentChat.id == message.chatId)
                {
                    message.date.ToLocalTime();
                    messages.Add(message);
                }
            }), e => Dispatcher.Invoke(async () => 
            {
                var chating = await API.Chats.GetChatInfo(account.accessToken, e);
                chats.Add(chating);
                listBox.Items.Add(chating.id);
            }));
            if (user.avatar != null)
            {
                image1.Source = new BitmapImage(new Uri(user.avatar));
            }
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var scrollView = FindScrollViewer(ListViewMes);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && scrollView.ScrollableHeight == scrollView.ContentVerticalOffset)
            {
                ListViewMes.ScrollIntoView(messages.Last());
            }
        }

        private void createChat_Click(object sender, RoutedEventArgs e)
        {
            //var users = new HashSet<ZeroAPI.User>();
            //foreach (var us in listBox3.Items)
            //{
            //    users.Add(ZeroAPI.User.GetInfo(int.Parse(us.ToString())));
            //}
            //user.CreateChat(textBox.Text, ZeroAPI.ChatType.Public, users);
            //searchUsers.Visibility = Visibility.Hidden;
            //BlurEffect blurEffect = new BlurEffect();
            //blurEffect.Radius = 0;
            //MainGrid.Effect = blurEffect;
            //MainGrid.IsHitTestVisible = true;
        }

        //loading chats
        private async void listBox_Loaded(object sender, RoutedEventArgs e)
        {
            long[] userChats = await API.Users.GetChats(account.accessToken);
            foreach (var i in userChats)
            {
                var chating = await API.Chats.GetChatInfo(account.accessToken, i);
                chats.Add(chating);
                listBox.Items.Add(chating.id);
            }
        }

        //Chose current chat
        private async void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                messages.Clear();
                var message = await API.Messages.GetMessages(account.accessToken, chats[listBox.SelectedIndex].id, 50, 1);
                if (message != null)
                {
                    foreach (var el in message)
                    {
                        Console.WriteLine(el.date.ToLocalTime());
                        messages.Add(el);
                    }
                }
                ListViewMes.ScrollIntoView(message.Last());
                currentChat = chats[listBox.SelectedIndex];
            }
        }

        //Send message via button
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileIds = "";
                foreach(var i in pictureIds)
                {
                    fileIds += i + ",";
                }
                if(fileIds != "")
                {
                    fileIds = fileIds.Remove(fileIds.Length - 1);
                }
                await API.Messages.SendMessage(account.accessToken, currentChat.id, textBox2.Text,fileIds);
                textBox2.Clear();
                pictureIds.Clear();
            }
            catch { }
            
        }

        //Search user
        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            listBox2.Items.Clear();
            users.Clear();
            var userIds = await API.Users.FindUsersByName(textBox3.Text, 10);
            if (userIds != null)
            {
                foreach (var user in userIds)
                {
                    users.Add(await API.Users.GetUserInfo(user));
                    var userSearched = await API.Users.GetUserInfo(user);
                    listBox2.Items.Add(userSearched.name);
                }
            }
        }

        //Open user information in search
        private void listBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                UserInformations.Visibility = Visibility.Visible;
                searchUsers.Visibility = Visibility.Hidden;
                NameOfSearchUser.Text = users[listBox2.SelectedIndex].name;
                if (user.avatar != null)
                {
                    image1.Source = new BitmapImage(new Uri(user.avatar));
                }
                //    listBox3.Items.Add(users[listBox2.SelectedIndex].Id);
            }
        }


        //Open search
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
            API.Connection.Disconnect();
        }

        //Close search
        private void image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listBox2.Items.Clear();
            listBoxPublic.Items.Clear();
            textBoxPublic.Clear();
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

        //Open bio
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 20;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = false;
            nameSetting.Text = user.name;
            if (user.avatar != null)
            {
                avatarSetting.Source = new BitmapImage(new Uri(user.avatar));
            }
            bioSetting.Text = user.description;
        }

        //Close user information in search
        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            UserInformations.Visibility = Visibility.Hidden;
            searchUsers.Visibility = Visibility.Visible;
        }

        //Create Dialog
        private void CreateChatOne_Click(object sender, RoutedEventArgs e)
        {
            API.Chats.CreateDialog(account.accessToken, users[listBox2.SelectedIndex].id);
            searchUsers.Visibility = Visibility.Hidden;
            UserInformations.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        //Close bio
        private void image1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 0;
            MainGrid.Effect = blurEffect;
            MainGrid.IsHitTestVisible = true;
        }

        //Open settings
        private void buttonSetting_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Hidden;
            AllSettings.Visibility = Visibility.Visible;
        }

        //Change new avatar
        private async void avatarSetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "jpg|*.jpg| bmp|*.bmp";

            Nullable<bool> result = dlg.ShowDialog();

            string filename = "";

            if (result == true)
            {
                filename = dlg.FileName;
            }
            var fileId = await API.Files.UploadFile(filename);
            long fileIds = Convert.ToInt32(fileId);
            await API.Users.ChangeAvatar(account.accessToken, fileIds);
            user = await API.Users.GetUserInfo(user.id);

            if (user.avatar != null)
            {
                image1.Source = new BitmapImage(new Uri(user.avatar));
            }
        }

        //Close settings
        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            NameChange.Text = "";
            OldPassword.Text = "";
            NewPassword.Text = "";
            Settings.Visibility = Visibility.Visible;
            AllSettings.Visibility = Visibility.Hidden;
        }

        //Save new password or new name
        private async void SaveChange_Click(object sender, RoutedEventArgs e)
        {
            if (NameChange.Text != "")
            {
                await API.Users.ChangeName(account.accessToken, NameChange.Text);
                user = await API.Users.GetUserInfo(account.userId);
                textBlock.Text = user.name;
                nameSetting.Text = user.name;
            }
            if (OldPassword.Text != "" && NewPassword.Text !="")
            {
                await API.Accounts.ChangePassword(account.accessToken, OldPassword.Text, NewPassword.Text);
            }
            NameChange.Text = "";
            OldPassword.Text = "";
            NewPassword.Text = "";
            Settings.Visibility = Visibility.Visible;
            AllSettings.Visibility = Visibility.Hidden;
        }

        //Change description
        private async void bioSetting_LostFocus(object sender, RoutedEventArgs e)
        {
            await API.Users.ChangeDescription(account.accessToken, bioSetting.Text);
            user = await API.Users.GetUserInfo(account.userId);
        }

        private void ListViewMes_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        //Send messages via enter key
        private async void textBox2_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            // Check if we have pressed enter
            if (e.Key == Key.Enter)
            {
                // If we have control pressed...
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // Add a new line at the point where the cursor is
                    var index = textBox2.CaretIndex;

                    // Insert the new line
                    textBox2.Text = textBox2.Text.Insert(index, Environment.NewLine);

                    // Shift the caret forward to the newline
                    textBox2.CaretIndex = index + Environment.NewLine.Length;

                    // Mark this key as handled by us
                    e.Handled = true;
                }
                else
                {
                    try
                    {
                        string fileIds = "";
                        foreach (var i in pictureIds)
                        {
                            fileIds += i + ",";
                        }
                        if (fileIds != "")
                        {
                            fileIds = fileIds.Remove(fileIds.Length - 1);
                        }
                        await API.Messages.SendMessage(account.accessToken, currentChat.id, textBox2.Text, fileIds);
                        textBox2.Clear();
                        pictureIds.Clear();
                    }
                    catch { }


                }

                // Mark the key as handled
                e.Handled = true;
            }
        }

        //Search public
        private async void FindPublic_Click(object sender, RoutedEventArgs e)
        {
            
            listBoxPublic.Items.Clear();
            if (textBoxPublic.Text != "")
            {
                foreach (var pub in await API.Chats.FindPublicByName(textBoxPublic.Text, 50))
                {
                    var publicSearched = await API.Chats.GetChatInfo(account.accessToken,pub);
                    listBoxPublic.Items.Add(publicSearched.name);
                }
            }
        }

        private void listBoxPublic_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        //Uploading file for messages
        private async void FilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "jpg|*.jpg| bmp|*.bmp";

            Nullable<bool> result = dlg.ShowDialog();

            string filename = "";

            if (result == true)
            {
                filename = dlg.FileName;
            }
            if (!String.IsNullOrWhiteSpace(filename))
            {
                var fileId = await API.Files.UploadFile(filename);
                if (fileId.HasValue)
                {
                    MessageBox.Show("Изображение добавлено");
                    pictureIds.Add(fileId.Value);
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить изображение");
                }
            }
        }

        private void CreatePublic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {

        }

        //Edit messages
        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        //Removing messages from you
        private async void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var message = (API.Messages)((StackPanel)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).GetBindingExpression(StackPanel.NameProperty).ResolvedSource;
            if (await API.Messages.DeleteMessage(account.accessToken, message.id, false))
            {
                messages.Remove(message);
            }
        }

        //Removing messages from all
        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var message = (API.Messages)((StackPanel)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).GetBindingExpression(StackPanel.NameProperty).ResolvedSource;
            if (await API.Messages.DeleteMessage(account.accessToken, message.id, true))
            {
                messages.Remove(message);
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
                return d as ScrollViewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var sw = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (sw != null) return sw;
            }
            return null;
        }
    }
}
