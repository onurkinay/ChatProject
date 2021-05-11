using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for Calling.xaml
    /// </summary>
    public partial class Calling : Window
    {
        Uye friend = null;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public Calling(Uye uye)
        {
            InitializeComponent();
            txtKisi.Content = uye.nickname;
            this.friend = uye;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("sohbetTalebiKabulu<"+friend.id);
            
            Ozel ozel = new Ozel(friend);
            ozel.gorusmeKabul();
            myWindow.ozelMesajlasmalar.Add(ozel);
            ozel.Show();
            this.Close();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("sohbetReddedildi<" + friend.id);
            this.Close();

        }
    }
}
