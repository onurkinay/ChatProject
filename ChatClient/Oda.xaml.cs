using Microsoft.Win32;
using System.Collections.Specialized;
using System.Windows;
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
        public Oda(sOda oda)
        {
            InitializeComponent();
            ((INotifyCollectionChanged)lbMesajlar.Items).CollectionChanged += ListView_CollectionChanged;
            this.id = oda.id;
            this.Title = "Oda#" + oda.id +  " "+oda.name;
        }

        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                myWindow.myClient.sendData(System.IO.File.ReadAllBytes(openFileDialog.FileName), openFileDialog.SafeFileName, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
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
                txtMesaj.Text = "";
            }
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            myWindow.katildigimOdalar.Remove(this);
            myWindow.myClient.sendMessage("odadanCikis<" + this.id);
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
