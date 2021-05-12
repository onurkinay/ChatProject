using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Ozel.xaml
    /// </summary>
    public partial class Ozel : Window
    {
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public bool isOpen = false;
        public Uye friend = null; 
        public Ozel(Uye uye)
        {
            InitializeComponent();
            this.friend = uye;
            this.Title = "Private Message: "+ friend.nickname;
             

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Gonder();
        } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            isOpen = false;
        }

        private void txtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Gonder();
            }
           
        } 
        void Gonder()
        {
            if (txtMesaj.Text != "")
            {
                myWindow.myClient.sendMessage("mesajVar<" + txtMesaj.Text + "<" + friend.id); 
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye(myWindow.myId, myWindow.myNickName), txtMesaj.Text), Background = Brushes.Blue });
                
                txtMesaj.Text = "";
            }
        }

        private void lbMesajlar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbMesajlar.SelectedItem != null)
            {
                MessageBox.Show(lbMesajlar.SelectedItem.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
