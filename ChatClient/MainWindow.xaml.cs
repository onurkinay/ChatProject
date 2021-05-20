using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public List<string> dosyaParcaciklari = new List<string>();
        public object[] fileItem = null;

        public ProgressBar yukleme = null;
        public MainWindow()
        {
            InitializeComponent(); 
        }

        public Uye getMyUye()
        {
            return new Uye(myId, myNickName);
        }
         

        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            connectServerWindow = new ConnectServer();
            connectServerWindow.ShowDialog(); 
        }

        private void btnOdaOlustur_Click(object sender, RoutedEventArgs e)
        {
            new CreateOda().ShowDialog();
        }

        private void lblClients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lblClients.SelectedItem != null)//hazır özel varsa yenisini oluşturma
            {
                foreach (Ozel ozel1 in ozelMesajlasmalar)
                {
                    if ((Uye)lblClients.SelectedItem == ozel1.friend)
                    {
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
            try
            {
                if (myClient != null)
                {
                    foreach (Ozel ozel in ozelMesajlasmalar) ozel.Close();
                    foreach (Oda oda in katildigimOdalar) oda.Close();

                    myClient.sendMessage("cikisYapiyorum");
                    myClient.client.Close();
                    Application.Current.Shutdown();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("client mainwindow hata:"+Ex.ToString());
                Application.Current.Shutdown();
            }
        }

        private void lbOdalar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbOdalar.SelectedItem != null)
            {
                foreach (Oda oda1 in katildigimOdalar)
                {
                    if (((classOda)lbOdalar.SelectedItem).id == oda1.id) {
                        oda1.Activate();
                        return;
                    }
                }
                myClient.sendMessage("odayaKatil<" + ((classOda)lbOdalar.SelectedItem).id);
                Oda oda = new Oda((classOda)lbOdalar.SelectedItem);
                oda.lbKatilimcilar.Items.Add("*you*");
                katildigimOdalar.Add(oda);
                oda.Show();
            }
        }

        private void downloadFile(object sender, RoutedEventArgs e)
        {
            if (yukleme != null)
            {
                MessageBox.Show("Aynı anda sadece bir dosya yükleyebilirsiniz. Dosya yükleyebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dosyaParcaciklari.Count == 0)
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

                    if (dosya.oda == null)
                        myClient.sendMessage("dosyaKabulu<file-" + myId + "-" + dosya.uye.id + "-" + dosya.dosyaId);
                    else myClient.sendMessage("dosyaKabulu<file-" + dosya.oda.id + "-" + dosya.uye.id + "-" + dosya.dosyaId);

                    buton.Visibility = Visibility.Collapsed;
                    ((ProgressBar)fileItem[0]).Visibility = Visibility.Visible;

                }
            }
            else
            {
                MessageBox.Show("Aynı anda sadece bir dosya indirebilirsiniz. Dosya indirebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
        private void ScrollViewer_Initialized(object sender, EventArgs e)
        { 
            ((ScrollViewer)sender).Width = ((ScrollViewer)sender).Width - 25;
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (sender is ScrollViewer && !e.Handled)

            {

                e.Handled = true;

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

                eventArg.RoutedEvent = UIElement.MouseWheelEvent;

                eventArg.Source = sender;

                var parent = ((Control)sender).Parent as UIElement;

                parent.RaiseEvent(eventArg);

            }

        }

        private void progress_Loaded(object sender, RoutedEventArgs e)
        { 
            yukleme = sender as ProgressBar;

            ListView item =
               
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(
               VisualTreeHelper.GetParent(yukleme)
               )))))))))))) as ListView;
     

            dosyaBilgileri ss = ((ListBoxItem)item.Items[item.Items.Count - 1]).Tag as dosyaBilgileri; 
            if (ss != null)
            { 
                string safeFileName = ss.safeFileName;
                string fileName = ss.fileName;
                
                if(ss.alici is Uye uye) 
                    myClient.sendData(safeFileName, fileName, uye);
                else if(ss.alici is Oda oda)
                    myClient.sendData(safeFileName, fileName, oda);
            }
        }
    
        public void PlaySound()
        {
            var uri = new Uri(@"when-604.wav", UriKind.RelativeOrAbsolute);
            var player = new MediaPlayer();

            player.Open(uri);
            player.Play();
        }

       
      
    }
}
