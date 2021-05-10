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
    /// Interaction logic for CreateOda.xaml
    /// </summary>
    public partial class CreateOda : Window
    {
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public CreateOda()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myWindow.myClient.sendMessage("odaOlustur<"+txtOda.Text);
             

            this.Close();
        }
    }
}
