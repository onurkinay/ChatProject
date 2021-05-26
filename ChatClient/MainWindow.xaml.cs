using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        public List<Uye> uyeler = new List<Uye>();

        public ConnectServer connectServerWindow = null;

        public string saveFilePath = "";
        public List<string> dosyaParcaciklari = new List<string>();
        public object[] fileItem = null;

        public ProgressBar yukleme = null;

        List<string> _List = Classes.EmojiList();


        public MainWindow()
        {
            InitializeComponent();
            btnOdaOlustur.IsEnabled = false;

            

            txtMesaj.Document.Blocks.Clear();
            FlowDocument mcFlowDoc = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(""));
            mcFlowDoc.Blocks.Add(para);
            txtMesaj.Document = mcFlowDoc;
            txtMesaj.AcceptsReturn = false;

            ((INotifyCollectionChanged)lbMesajlar.Items).CollectionChanged += ListView_CollectionChanged;
        }

        public Uye getMyUye()
        {
            return new Uye(myId, myNickName);
        }
         
        private void btnBaglan_Click(object sender, RoutedEventArgs e)
        {
            if (btnConnect.Tag.ToString() == "ayril")
            {
                if (MessageBox.Show("Sunucudan ayrılacaksınız. Emin misiniz", "Uyarı!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ayril();

                    btnConnect.Header = "Sunucuya Bağlan";
                    btnConnect.Tag = "baglan";
                    btnConnect.IsEnabled = true;
                    btnOdaOlustur.IsEnabled = false;
                    Title = "Chat Client";
                }
            }
            else
            {
                connectServerWindow = new ConnectServer();
                connectServerWindow.ShowDialog();
            }
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
            ayril(); 
            Application.Current.Shutdown();
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

        private void ayril()
        {
            try
            {
                if (myClient != null)
                {
                    foreach (Ozel ozel in ozelMesajlasmalar) ozel.Close();
                    foreach (Oda oda in katildigimOdalar) oda.Close();
                    lblClients.Items.Clear();
                    lbOdalar.Items.Clear();
                    lbMesajlar.Items.Clear();

                    txtMesaj.IsEnabled = false;
                    btnGonder.IsEnabled = false;

                    txtId.Text = "";
                    uyeler.Clear();
                    myClient.sendMessage("cikisYapiyorum");
                    myClient.client.Close();
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("client mainwindow hata:" + Ex.ToString()); 
            }
        }

        private void downloadFile(object sender, RoutedEventArgs e)
        {
            if (yukleme != null)
            {
                MessageBox.Show("Dosya indirirken yükleme yapamazsınız. Dosya yükleyebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    saveFilePath = saveFileDialog.FileName;

                    if (dosya.odaId == null)
                        myClient.sendMessage("dosyaKabulu<file-" + myId + "-" + dosya.uye.id + "-" + dosya.dosyaId);
                    else myClient.sendMessage("dosyaKabulu<file-" + dosya.odaId + "-" + dosya.uye.id + "-" + dosya.dosyaId);

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
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = MouseWheelEvent,
                    Source = sender
                };
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
                else if(ss.alici is MainWindow mw)
                    myClient.sendData(safeFileName, fileName, mw);
            }
        }
    
        public void PlaySound()
        {
            var uri = new Uri(@"when-604.wav", UriKind.RelativeOrAbsolute);
            var player = new MediaPlayer();

            player.Open(uri);
            player.Play();
        }


        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            if (dosyaParcaciklari.Count != 0)
            {
                MessageBox.Show("Dosya indirirken yükleme yapamazsınız.", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (yukleme != null)
            {
                MessageBox.Show("Aynı anda sadece bir dosya yükleyebilirsiniz. Dosya yükleyebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(getMyUye(), "###dosyaVar###dosyaAdi=" + openFileDialog.SafeFileName + "*-1", "0"), Tag = new dosyaBilgileri(openFileDialog.SafeFileName, openFileDialog.FileName, this) });
        }

        private void txtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                Gonder();
        }

        void Gonder(string dosya = "")
        {
            Console.WriteLine(ConvertRichTextBoxContentsToString(txtMesaj));
            if (ConvertRichTextBoxContentsToString(txtMesaj) != "" || dosya != "")
            {
                string str = (dosya == "") ? ConvertRichTextBoxContentsToString(txtMesaj) : dosya;
                var charsToRemove = new string[] { "<", "~" };//sunucuya gönderilirken kullanılan ayırıcı karakterlerin kullanımı engeller
                foreach (var c in charsToRemove)
                    str = str.Replace(c, string.Empty);

                myClient.mesajGonder("odayaMesajAt<0<" + str);
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(getMyUye(), ConvertRichTextBoxContentsToString(txtMesaj).Replace("\r\n", ""), "0") });

                txtMesaj.Text = ""; 
            }
        }
        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                lbMesajlar.ScrollIntoView(e.NewItems[0]);
            }
        }

        private void txtMesaj_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConvertRichTextBoxContentsToString((RichTextBox)sender).Length > 1)
            {
                TextPointer tp = txtMesaj.Document.Blocks.FirstBlock.ContentEnd.GetPositionAtOffset(-5);
                if (tp != null)
                {
                    string _Text = new TextRange(tp, txtMesaj.Document.Blocks.FirstBlock.ContentEnd).Text;
                    for (int count = 0; count < _List.Count; count++)
                    {
                        string[] _Split = _List[count].Split(',');
                        _Text = _Text.Replace(_Split[0], _Split[1]);
                    }
                    if (_Text != new TextRange(tp, txtMesaj.Document.Blocks.FirstBlock.ContentEnd).Text)
                    {
                        new TextRange(tp, txtMesaj.Document.Blocks.FirstBlock.ContentEnd).Text = _Text;
                    }
                    Block blk = txtMesaj.Document.Blocks.FirstBlock;
                    txtMesaj.CaretPosition = blk.ElementEnd;
                }
            }

        }
        string ConvertRichTextBoxContentsToString(RichTextBox rtb)
        {
            if (rtb.Document.Blocks.FirstBlock != null)
            {
                return txtMesaj.Text;
            }
            return "";
        }
    }
}
