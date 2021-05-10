using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows; 
using System.Collections.Generic;

namespace ChatServer
{
  

    /// <summary>
    /// 
    /// </summary>
    class Server
    {
        TcpListener server = null;
        MainWindow myWindow = null;
        List<Client> clientLists = new List<Client>();
        List<Oda> odalarLists = new List<Oda>();
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
                    Client newUser = new Client(client);

                  
                   
                    sendClientMessage(newUser.id+"~"+connectingClient(), newUser,false);//yeni bağlanan client'a sunucuda bulunan odalar ve üyeleri gönderir
                    sendClientMessage("yeniUye=" + newUser.id + "<" + newUser.nickname, newUser,true);//sunucuya bağlı olan bütün üyelere yeni clienti bildirir

                    Console.WriteLine("Connected!");
                    clientLists.Add(newUser);
                    addClientToList(newUser);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(newUser);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void sendClientMessage(string str, Client client, bool broadcast)
        {
            try
            {
                if (!broadcast)
                {
                    var stream = client.user_tcpclient.GetStream();

                    Byte[] reply = Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                }
                else
                {
                    foreach (Client uye in clientLists)
                    {
                        if (uye != client)
                        {
                            var stream = uye.user_tcpclient.GetStream();

                            Byte[] reply = Encoding.ASCII.GetBytes(str);
                            stream.Write(reply, 0, reply.Length);
                        }
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Server'den hata");
            }
        }

       

        public void HandleDeivce(Object obj)
        {
            
            TcpClient client = (TcpClient) ((Client)obj).user_tcpclient;
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
                    Console.WriteLine("{1}: Received: {0} in Server from "+ ((Client)obj).id, data, Thread.CurrentThread.ManagedThreadId);


                    if (data.Contains("sohbetBaslat"))
                    {
                        string chatFriend = data.Split('<')[1];
                        Console.WriteLine("sohbet talebi var");
                        foreach(Client friend in clientLists)
                        {
                            if(friend.id.ToString() == chatFriend)
                            {
                                sendClientMessage("sohbetTalebiVar<"+ ((Client)obj).id, friend,false);
                            }
                        }
                       
                    }else if (data.Contains("sohbetTalebiKabulu"))
                    {
                        string chatFriend = data.Split('<')[1];
                        Console.WriteLine("sohbet talebi kabul edildi");
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == chatFriend)
                            {
                                sendClientMessage("sohbetTalebiKabulEdildi<" + ((Client)obj).id, friend, false);
                            }
                        }
                    }
                    else if (data.Contains("sohbetReddedildi"))
                    {
                        string chatFriend = data.Split('<')[1];
                        Console.WriteLine("sohbet talebi reddedildi");
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == chatFriend)
                            {
                                sendClientMessage("sohbetTalebiReddedildi<" + ((Client)obj).id, friend, false);
                            }
                        }
                    }
                    else if (data.Contains("mesajVar"))
                    {
                        string mesaj = data.Split('<')[1];
                        string alici = data.Split('<')[2];
                        Console.WriteLine("özel mesaj var");
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == alici)
                            {
                                sendClientMessage("mesajAliciya<" + ((Client)obj).id+"<"+mesaj, friend, false);
                            }
                        }
                    }
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
        public void addClientToList(Client client)
        {
            Application.Current.Dispatcher.Invoke(delegate {
                myWindow.lblClients.Items.Add(client);
            });
        }

        public string connectingClient()//bir client bağlandığında serverda bulunan odalar, üyeler fln gönder// oda ismi/id 
        {
            string uyeler = "yeniBaglananlar{";
            foreach(Client client in clientLists)
            {
                uyeler += "["+client.id + "<" +client.nickname+""; // 
            }
            uyeler += "}";

            string odalar = "{";
            foreach (Oda oda in odalarLists)
            {
                odalar += "[" + oda.id + "<" + oda.name + ""; //  
            }
            odalar += "}";

            return uyeler + odalar;
        }
    }
}
