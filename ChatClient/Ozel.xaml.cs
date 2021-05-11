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

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Ozel.xaml
    /// </summary>
    public partial class Ozel : Window
    {
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
       
        public Uye friend = null;
        public Ozel(Uye uye)
        {
            InitializeComponent();
            this.friend = uye;
            this.Title = "Private Message: "+ friend.id;

            this.friend = uye;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("mesajVar<"+txtMesaj.Text+"<"+friend.id);
            lbMesajlar.Items.Add(myWindow.txtId.Text+": " +txtMesaj.Text);
            txtMesaj.Text = "";

        }

        public void gorusmeKabul()
        {
            btnGonder.IsEnabled = true;
            this.Title = friend.nickname+" ile özel görüşme";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(friend.id == -1)
            {
                MessageBox.Show("Görüşme talebi reddedildi");
            }
            else
            {//sohbetten ayrılma
                myWindow.myClient.sendMessage("sohbettenAyrildi<"+ friend.id);
            }
        }
    }
}
