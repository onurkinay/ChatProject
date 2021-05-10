using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Windows; 
using System.Collections.Generic;
namespace ChatClient
{
   
    // State object for receiving data from remote device.  
   public class Client
    {
         TcpClient client = null;
         NetworkStream stream = null;
         MainWindow myWindow = null;
        List<Uye> uyeler = new List<Uye>();

        public Client(MainWindow cmyWindow)
        {
            myWindow = cmyWindow;
        }
        public void Connect(String server, String message)
        {
           
            try
            {
                Int32 port = 13000;
                client = new TcpClient(server, port);

                stream = client.GetStream();

                int count = 0;
                while (count++ < 3)
                {
                    
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);//ben bağlandım bana serverdan bilgi getir
                    Console.WriteLine("Sent: {0}", message);


                    // Bytes Array to receive Server Response.
                    //data = new Byte[256];
                    //String response = String.Empty;

                    //// Read the Tcp Server Response Bytes.
                    //Int32 bytes = stream.Read(data, 0, data.Length);
                    //response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    //Console.WriteLine("Received: {0}", response);

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(stream);
                     
                }

                //stream.Close();
                //client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }

            Console.Read();
        }

        public void HandleDeivce(Object obj)
        {

            var stream = (NetworkStream)obj;
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
                    Console.WriteLine("{1}: Received: {0} in Client", data, Thread.CurrentThread.ManagedThreadId);
                    if (data.Contains("yeniBaglananlar"))
                    {
                        yeniGelen(data);
                    }else if (data.Contains("yeniUye="))
                    {
                        string gelen = data.Remove(0, 8);//yeniUye=
                        string[] uye_bilgileri = gelen.Split('<');
                        Uye eklenecekUye = new Uye();
                        eklenecekUye.id = Convert.ToInt32(uye_bilgileri[0]);
                        eklenecekUye.nickname = uye_bilgileri[1];
                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                        Application.Current.Dispatcher.Invoke(delegate {
                            myWindow.lblClients.Items.Add(eklenecekUye);
                        });
                       
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

        public void sendMessage(string message)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        public void updateUI(string data )
        {
            Application.Current.Dispatcher.Invoke(delegate {
               // myWindow.txtBlock.Text = data;
            });
        }

        public void yeniGelen(string data)
        {
            try {
                data = data.Remove(0, 15);//yeniBaglananlar
                string[] veri = data.Split('}');

                for (int i = 0; i < veri.Length; i++)
                {
                    if (veri[i] == "{")
                    {
                        Console.WriteLine(((i == 1) ? "Odalar" : "Uyeler") + " hakkında boş bilgi");
                    }
                    else
                    { 
                        if (veri[i].Length > 1)
                        {
                            string gelen = veri[i].Remove(0, 1);
                            if (i == 0)//uyelerin duzenlenmesi
                            {
                                string[] uyeler = gelen.Split('[');
                                foreach (string input in uyeler)
                                {
                                    if (input.Contains("<"))
                                    {
                                        string[] uye_bilgileri = input.Split('<');
                                        Uye eklenecekUye = new Uye();
                                        eklenecekUye.id = Convert.ToInt32(uye_bilgileri[0]);
                                        eklenecekUye.nickname = uye_bilgileri[1];
                                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                                        Application.Current.Dispatcher.Invoke(delegate {
                                            myWindow.lblClients.Items.Add(eklenecekUye);
                                        });
                                    }
                                }
                            }

                        }
                    }
                }

            }catch(System.IO.IOException e)
            {
                Console.WriteLine("bilgileriDegerlendir'den hata");
            }
            }
    }
    
    
}
