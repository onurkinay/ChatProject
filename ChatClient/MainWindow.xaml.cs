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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client myClient = null;
        public MainWindow()
        {
            InitializeComponent();
        }
         
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           // myClient.sendMessage(txtBox.Text);
        }
         

        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            MainWindow myWindow = null;
         
            this.Dispatcher.Invoke((Action)(() =>
            {
                // box = txtBox.Text;
                myWindow = Application.Current.MainWindow as MainWindow;
            }));
            myClient = new Client(myWindow);
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                myClient.Connect("127.0.0.1", "hello");
            }).Start();
            btnConnect.IsEnabled = false;
        }
    }
}
