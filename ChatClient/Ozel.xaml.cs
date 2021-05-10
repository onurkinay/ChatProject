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
        public int id = 0;
        public Ozel(int chatFriend)
        {
            InitializeComponent();
           
            btnGonder.IsEnabled = false;
            
            this.id = chatFriend;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("mesajVar<"+txtMesaj.Text+"<"+id);
            lbMesajlar.Items.Add(myWindow.txtId.Text+": " +txtMesaj.Text);
            txtMesaj.Text = "";

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(id == -1)
            {
                MessageBox.Show("Görüşme talebi reddedildi");
            }
            else
            {//sohbetten ayrılma
                myWindow.myClient.sendMessage("sohbettenAyrildi<"+ id);
            }
        }
    }
}
