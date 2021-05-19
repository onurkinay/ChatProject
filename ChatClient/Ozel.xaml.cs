using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Ozel.xaml
    /// </summary>
    public partial class Ozel : Window
    {
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public bool isOpen = false;
        public Uye friend = null;
        List<string> _List = Classes.EmojiList();
        public Ozel(Uye uye)
        {
            InitializeComponent();
            ((INotifyCollectionChanged)lbMesajlar.Items).CollectionChanged += ListView_CollectionChanged;
 
            this.friend = uye;
            this.Title = "Private Message: " + friend.nickname;


            txtMesaj.Document.Blocks.Clear();

            FlowDocument mcFlowDoc = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Add(new Run(""));
            mcFlowDoc.Blocks.Add(para);
            txtMesaj.Document = mcFlowDoc;
            txtMesaj.AcceptsReturn = false;

           
            



        }

        private void Button_Click(object sender, RoutedEventArgs e)
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
            {
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye(myWindow.myId, myWindow.myNickName), "###dosyaVar###dosyaAdi=" + openFileDialog.SafeFileName + "*-1"), Tag = new dosyaBilgileri(openFileDialog.SafeFileName, openFileDialog.FileName, friend) });

            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { 
            e.Cancel = true; 
            this.Visibility = Visibility.Hidden;
            isOpen = false;
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
                myWindow.myClient.sendMessage("mesajVar<" + str + "<" + friend.id);
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye(myWindow.myId, myWindow.myNickName), str) });

                txtMesaj.Text = "";
            }
        }

        private void lbMesajlar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbMesajlar.SelectedItem != null)
            {
                MessageBox.Show(lbMesajlar.SelectedItem.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
        private void txtMesaj_TextChanged(object sender, TextChangedEventArgs e)
        {


            if (ConvertRichTextBoxContentsToString((RichTextBox)sender).Length > 1)
            { 
                TextPointer tp = txtMesaj.Document.Blocks.FirstBlock.ContentEnd.GetPositionAtOffset(-4);
                if (tp != null)
                {
                    string _Text = new TextRange(tp, txtMesaj.Document.Blocks.FirstBlock.ContentEnd).Text;
                    for (int count = 0; count < _List.Count; count++)
                    {
                        string[] _Split = _List[count].Split(','); //Separate each string in _List[count] based on its index
                        _Text = _Text.Replace(_Split[0], _Split[1]); //Replace the first index with the second index
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
