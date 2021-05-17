using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
           /* Console.WriteLine("CHAOS EXIT");
            MainWindow myWindow = Application.Current.MainWindow as MainWindow;
            myWindow.myClient.sendMessage("cikisYapiyorum");//null hatası
            myWindow.myClient.client.Close();
            Environment.Exit(4);
           */
            e.Handled = true;
        }
    }

   
}
