using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Collections;

namespace ChatServer
{
  

    class Server
    {
        TcpListener server = null;
        MainWindow myWindow = null;
        ArrayList clientLists = new ArrayList();
        public Server(string ip, int port, MainWindow cmyWindow)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            myWindow = cmyWindow;
            server.Start();
        }

        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    clientLists.Add(client);
                    addClientToList((clientLists.Count-1).ToString());
                    Console.WriteLine("Connected!");

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void sendClientMessage(string str, int id)
        {
            TcpClient client = (TcpClient)clientLists[id];
            var stream = client.GetStream();
            
            Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
            stream.Write(reply, 0, reply.Length);

        }

       

        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);


                    updateUI(data);
                    //string str = "Hey Device!";
                    //Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    //stream.Write(reply, 0, reply.Length);
                    //Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        public void updateUI(string data)
        { 
            Application.Current.Dispatcher.Invoke(delegate {
                myWindow.txtReturn.Text = data;
            });
        }
        public void addClientToList(string id)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                myWindow.lblClients.Items.Add(id);
            });
        }
    }
}
