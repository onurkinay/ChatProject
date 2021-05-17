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
            if (btnServer.Content.ToString() != "Sunucuyu Durdur")
            {
                try
                {  
                    myserver = new Server( !(bool)cbInternet.IsChecked , 13000);

                    Thread t = new Thread(delegate ()
                    {
                        // replace the IP with your system IP Address...
                        myserver.StartListener();


                    });
                    t.Start();

                    Console.WriteLine("Server Started...!");
                    btnServer.Content = "Sunucuyu Durdur";
                    cbInternet.IsEnabled = false;
                }
                catch (SocketException err)
                {
                    MessageBox.Show("Sunucu başlatılamadı. Daha fazla bilgi için Hata.txt dosyasına bakınız", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("SocketException: {0}", err);

                }
            }
            else
            {
                if( MessageBox.Show("Sunucu kapatılacak. Emin misiniz","Uyarı!",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    myserver.sendClientMessage("###serverKapatildi###", null, true);//server kapatıldı
                    btnServer.Content = "Sunucuyu Başlat";
                    cbInternet.IsEnabled = true;
                    myserver.server.Stop();
                    myserver = null;
                }
            
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Client selectedClient = (Client)lblClients.SelectedItem;
         
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myserver.sendClientMessage("###serverKapatildi###",null,true);//server kapatıldı
            myserver.server.Stop();//server durdurulmalı
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if(lbOdalar.SelectedItem != null)
            {
                myserver.sendClientMessage("buOdaKaldirdim<"+ ((Oda)lbOdalar.SelectedItem).id,null,true); 
                myserver.odalarLists.Remove((Oda)lbOdalar.SelectedItem); 
                lbOdalar.Items.Remove(lbOdalar.SelectedItem); 
            }
        }
    }
}
