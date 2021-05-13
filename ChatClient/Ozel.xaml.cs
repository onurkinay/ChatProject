﻿using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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
        public Ozel(Uye uye)
        {
            InitializeComponent();
            this.friend = uye;
            this.Title = "Private Message: "+ friend.nickname;
             

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                     Gonder("###dosyaVar###dosyaAdi=" + openFileDialog.SafeFileName);
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
        void Gonder(string dosya="")
        {
            if (txtMesaj.Text != "" || dosya!="")
            {
                string str = (dosya=="") ? txtMesaj.Text : dosya;
                var charsToRemove = new string[] { "<", "~" };
                foreach (var c in charsToRemove)
                {
                    str = str.Replace(c, string.Empty);
                }
                myWindow.myClient.sendMessage("mesajVar<" + str + "<" + friend.id); 
                lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye(myWindow.myId, myWindow.myNickName), str), Background = Brushes.SkyBlue });
                
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
    }
}
