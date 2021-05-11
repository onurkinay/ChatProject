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
        public Client myClient = null;
        public List<Ozel> ozelMesajlasmalar = new List<Ozel>();
        public List<Oda> katildigimOdalar = new List<Oda>();
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
                myClient.Connect("192.168.1.101");
            }).Start();
            btnConnect.IsEnabled = false;
        }

        private void btnOdaOlustur_Click(object sender, RoutedEventArgs e)
        {
            new CreateOda().Show();
        }

        private void lblClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lblClients.SelectedItem != null)
            {
                myClient.sendMessage("sohbetBaslat<" + ((Uye)lblClients.SelectedItem).id);
                Ozel ozel = new Ozel( ((Uye)lblClients.SelectedItem).id);
                ozelMesajlasmalar.Add(ozel);
                ozel.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myClient.sendMessage("cikisYapiyorum");
        }

        private void lbOdalar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbOdalar.SelectedItem != null)
            {
                myClient.sendMessage("odayaKatil<" + ((sOda)lbOdalar.SelectedItem).id);
                Oda oda = new Oda(((sOda)lbOdalar.SelectedItem).id);
                oda.lbKatilimcilar.Items.Add("*you*");
                katildigimOdalar.Add(oda);
                oda.Show();
            }
        }
    }
}
