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

            this.id = oda.id;
            this.Title = "Oda#" + oda.id +  " "+oda.name;
        }

        private void btnGonder_Click(object sender, RoutedEventArgs e)
        {
            Gonder();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myWindow.myClient.sendMessage("odadanCikis<" + this.id);
        }

        private void txtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Gonder();
            }
        }

        void Gonder()
        {
            myWindow.myClient.sendMessage("odayaMesajAt<" + this.id + "<" + txtMesaj.Text);
            txtMesaj.Text = "";
        }
    }
}
