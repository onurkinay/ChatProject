using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for ConnectServer.xaml
    /// </summary>
    public partial class ConnectServer : Window
    {

        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public ConnectServer()
        {
            InitializeComponent();
            lbStatus.Visibility = Visibility.Hidden;
        }

        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            lbStatus.Visibility = Visibility.Visible;
            btnBaglan.IsEnabled = false;
            string ip = "127.0.0.1";
            if (cbServer.Text != "")
            {
                ip = cbServer.Text;
            }
            myWindow.myClient = new Client(this);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                myWindow.myClient.Connect(ip);
            }).Start();



        }

        private void btnKabul_Click(object sender, RoutedEventArgs e)
        {
            
            if (txtNickname.Text != "")
            {
                myWindow.myClient.sendMessage("YeniNickName<" + txtNickname.Text); 
                myWindow.btnConnect.IsEnabled = false;
                btnKabul.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Lütfen bir nickname giriniz", "Boş nickname", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(myWindow.myClient != null && btnKabul.IsEnabled)
            {
                myWindow.myClient.sendMessage("cikisYapiyorum");
                myWindow.myClient.client.Close();
                myWindow.myClient = null;
            }
        }
    }
}
