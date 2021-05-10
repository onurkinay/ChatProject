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

namespace ChatServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server myserver = null;
        public MainWindow()
        {
            InitializeComponent();
         
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            btnServer.IsEnabled = false;
            MainWindow myWindow = null;
            this.Dispatcher.Invoke((Action)(() =>
            {//this refer to form in WPF application 
                myWindow = Application.Current.MainWindow as MainWindow;
            }));
            myserver = new Server("127.0.0.1", 13000, myWindow);
         
            Thread t = new Thread(delegate ()
            {
                // replace the IP with your system IP Address...
                myserver.StartListener();

              
            });
            t.Start();

            Console.WriteLine("Server Started...!");
             
        }

        public void getData(string Data)
        {
            txtReturn.Text = Data;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Client selectedClient = (Client)lblClients.SelectedItem;
            myserver.sendClientMessage(txtBox.Text, selectedClient,false );
        }
    }
}
