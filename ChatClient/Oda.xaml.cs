using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Oda.xaml
    /// </summary>
    public partial class Oda : Window
    {
        public int id = 0;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public Oda(classOda oda)
        {
            InitializeComponent();
            ((INotifyCollectionChanged)lbMesajlar.Items).CollectionChanged += ListView_CollectionChanged;
            this.id = oda.id;
            this.Title = "Oda#" + oda.id +  " "+oda.name;
        }

        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            if (myWindow.yukleme == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                    lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), "###dosyaVar###dosyaAdi=" + openFileDialog.SafeFileName, this), Tag = new dosyaBilgileri(openFileDialog.SafeFileName, openFileDialog.FileName, this) });

                //değiştirilmeli
            }
            else
            {
                MessageBox.Show("Aynı anda sadece bir dosya yükleyebilirsiniz. Dosya yükleyebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myWindow.katildigimOdalar.Remove(this);
            myWindow.myClient.sendMessage("odadanCikis<" + this.id);
        }

        private void txtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Gonder();
            }
        }

        void Gonder(string dosya = "")
        {
            if (txtMesaj.Text != "" || dosya != "")
            {
                string str = (dosya == "") ? txtMesaj.Text : dosya;
                var charsToRemove = new string[] { "<", "~" };
                foreach (var c in charsToRemove)
                {
                    str = str.Replace(c, string.Empty);
                }
                myWindow.myClient.sendMessage("odayaMesajAt<" + this.id + "<" + str);
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), txtMesaj.Text, this) });

                txtMesaj.Text = "";
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
          
        }

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // scroll the new item into view   
                lbMesajlar.ScrollIntoView(e.NewItems[0]);
            }
        }
    }
}
