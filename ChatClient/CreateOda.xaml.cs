using System.Windows;

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
