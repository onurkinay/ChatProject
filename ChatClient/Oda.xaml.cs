using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Collections.Generic;


namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Oda.xaml
    /// </summary>
    public partial class Oda : Window
    {
        public int id = 0;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        List<string> _List = Classes.EmojiList();

        public Oda(classOda oda)
        {
            InitializeComponent();
            ((INotifyCollectionChanged)lbMesajlar.Items).CollectionChanged += ListView_CollectionChanged;
            this.id = oda.id;
            this.Title = "Oda #" + oda.id +  " "+oda.name;

            txtMesaj.Document.Blocks.Clear();
            FlowDocument mcFlowDoc = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(""));
            mcFlowDoc.Blocks.Add(para);
            txtMesaj.Document = mcFlowDoc;
            txtMesaj.AcceptsReturn = false;
            

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myWindow.katildigimOdalar.Remove(this);
            myWindow.myClient.sendMessage("odadanCikis<" + this.id);
        }
        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            if (myWindow.dosyaParcaciklari.Count != 0)
            {
                MessageBox.Show("Dosya indirirken yükleme yapamazsınız.", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (myWindow.yukleme != null)
            {
                MessageBox.Show("Aynı anda sadece bir dosya yükleyebilirsiniz. Dosya yükleyebilmek için önceki işlemin bitmesini bekleyin", "Dosya Yükleme", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), "###dosyaVar###dosyaAdi=" + openFileDialog.SafeFileName + "*-1", "0"), Tag = new dosyaBilgileri(openFileDialog.SafeFileName, openFileDialog.FileName, this) });
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
             
                myWindow.myClient.mesajGonder("odayaMesajAt<" + this.id + "<" + str);
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), ConvertRichTextBoxContentsToString(txtMesaj).Replace("\r\n", ""), "0") });

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
