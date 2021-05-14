using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public string saveFilePath = "";
        public string dosyaParcaciklari = "";
        public object[] fileItem = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        public Uye getMyUye()
        {
            return new Uye(myId, myNickName);
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

                foreach (Ozel ozel1 in ozelMesajlasmalar)
                {
                    if ((Uye)lblClients.SelectedItem == ozel1.friend)
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
                foreach (Oda oda1 in katildigimOdalar)
                {
                    if (((sOda)lbOdalar.SelectedItem).id == oda1.id) {
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

        private void downloadFile(object sender, RoutedEventArgs e)
        {
            fileItem = new object[2];
            Button buton = sender as Button;
            Message dosya = ((Button)sender).Tag as Message;

             

            fileItem[0] = MyClass.GetMyProperty(buton) as ProgressBar;
            fileItem[1] = buton;
            Console.WriteLine("download file " + dosya.mesaj);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = dosya.mesaj;
            if (saveFileDialog.ShowDialog() == true)
            {
                dosya.mesaj = "###dosyaAliniyor###";
                saveFilePath = saveFileDialog.FileName;

                if(dosya.oda == null)
                    myClient.sendMessage("dosyaKabulu<file-" + myId + "-" + dosya.uye.id);
                else myClient.sendMessage("dosyaKabulu<file-" + dosya.oda.id + "-" + dosya.uye.id);

                buton.Visibility = Visibility.Collapsed;
                ((ProgressBar)fileItem[0]).Visibility = Visibility.Visible;

            }


        }
    }
}
