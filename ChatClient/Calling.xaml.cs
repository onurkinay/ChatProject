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
        int id = 0;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public Calling(int id)
        {
            InitializeComponent();
            txtKisi.Content = id;
            this.id = id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("sohbetTalebiKabulu<"+id);
            
            Ozel ozel = new Ozel( id );
            ozel.btnGonder.IsEnabled = true;
            ozel.Title = "Özel görüşme - " + id;
            myWindow.ozelMesajlasmalar.Add(ozel);
            ozel.Show();
            this.Close();
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("sohbetReddedildi<" + id);
            this.Close();

        }
    }
}
