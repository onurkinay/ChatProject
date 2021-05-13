using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string myId = null;
        public string myNickName = null;
        public Client myClient = null;
        public List<Ozel> ozelMesajlasmalar = new List<Ozel>();
        public List<Oda> katildigimOdalar = new List<Oda>();
        public ConnectServer connectServerWindow = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        public Uye getMyUye()
        {
            return new Uye( myId,myNickName );
        }
         
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           // myClient.sendMessage(txtBox.Text);
        }
         

        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            connectServerWindow = new ConnectServer();
            connectServerWindow.Show();
         
        
        }

        private void btnOdaOlustur_Click(object sender, RoutedEventArgs e)
        {
            new CreateOda().Show();
        }

        private void lblClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lblClients.SelectedItem != null)//hazır özel varsa yenisini oluşturma
            {
                 
                foreach(Ozel ozel1 in ozelMesajlasmalar)
                {
                    if( (Uye)lblClients.SelectedItem == ozel1.friend)
                    {
                         
                       // myClient.sendMessage("sohbetBaslat<" + ((Uye)lblClients.SelectedItem).id);
                        ozel1.isOpen = true;
                        ozel1.Visibility = Visibility.Visible;
                        return;
                    }
                }
                myClient.sendMessage("sohbetBaslat<" + ((Uye)lblClients.SelectedItem).id);
                Ozel ozel = new Ozel((Uye)lblClients.SelectedItem);
                ozelMesajlasmalar.Add(ozel);
                ozel.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myClient.sendMessage("cikisYapiyorum");//null hatası
            myClient.client.Close();
        }

        private void lbOdalar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbOdalar.SelectedItem != null)
            {
                foreach(Oda oda1 in katildigimOdalar)
                {
                    if( ((sOda)lbOdalar.SelectedItem).id == oda1.id ){
                        oda1.Activate();
                        return;
                    }
                }
                myClient.sendMessage("odayaKatil<" + ((sOda)lbOdalar.SelectedItem).id);
                Oda oda = new Oda((sOda)lbOdalar.SelectedItem);
                oda.lbKatilimcilar.Items.Add("*you*");
                katildigimOdalar.Add(oda);
                oda.Show();
            }
        }
       
    }
}
