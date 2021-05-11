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
        }

        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            
            myWindow.myClient = new Client(myWindow);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                myWindow.myClient.Connect("127.0.0.1");
            }).Start();
            

        }

        private void btnKabul_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("YeniNickName<"+txtNickname.Text);
            myWindow.Title = "Nickname: "+txtNickname.Text;
            myWindow.btnConnect.IsEnabled = false;
            this.Close();
        }
    }
}
