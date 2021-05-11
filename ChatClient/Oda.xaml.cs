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
    /// Interaction logic for Oda.xaml
    /// </summary>
    public partial class Oda : Window
    {
        public int id = 0;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public Oda(int id)
        {
            this.id = id;
            this.Title = "Oda#" + id;
            InitializeComponent();
        }

        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("odayaMesajAt<"+this.id+"<"+txtMesaj.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myWindow.myClient.sendMessage("odadanCikis<" + this.id);
        }
    }
}
