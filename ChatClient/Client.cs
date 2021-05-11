using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Windows; 
using System.Collections.Generic;
using System.IO;

namespace ChatClient
{
   
    // State object for receiving data from remote device.  
   public class Client
    {
         public TcpClient client = null;
         NetworkStream stream = null;
         MainWindow myWindow = null;
        public int id = -1;
        List<Uye> uyeler = new List<Uye>();

        public Client(MainWindow cmyWindow)
        {
            myWindow = cmyWindow;
        }
        public void Connect(String server)
        {
           
            try
            {
                Int32 port = 13000;
                client = new TcpClient(server, port);

                stream = client.GetStream();

                int count = 0;
                while (count++ < 3)
                {
                    
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes("connectMe");

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);//ben bağlandım bana serverdan bilgi getir
                    


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
                        string[] gelen = data.Split('~');
                        id = Convert.ToInt32(gelen[0]);
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.txtId.Text = id.ToString();
                        });
                        yeniGelen(gelen[1]);
                    }
                    else if (data.Contains("yeniUye="))
                    {
                        string gelen = data.Remove(0, 8);//yeniUye=
                        string[] uye_bilgileri = gelen.Split('<');
                        Uye eklenecekUye = new Uye(Convert.ToInt32(uye_bilgileri[0]), uye_bilgileri[1]);
                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lblClients.Items.Add(eklenecekUye);
                        });

                    }
                    else if (data.Contains("sohbetTalebiVar"))
                    {
                        Console.WriteLine(data.Split('<')[1] + " kişi görüşmek istiyor");

                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach(Uye uye in myWindow.lblClients.Items)
                            {
                                if(uye.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    Calling calling = new Calling(uye);
                                    calling.Show();
                                    break;
                                }
                            }
                         
                        });

                    }
                    else if (data.Contains("sohbetTalebiKabulEdildi"))
                    {
                        Console.WriteLine("Görüşme başlatıldı");


                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    ozel.gorusmeKabul();
                                    
                                }
                            }

                        });

                    }
                    else if (data.Contains("sohbetTalebiReddedildi"))
                    {
                        Console.WriteLine("Görüşme Reddedildi");
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Ozel silinecekOzel = null;
                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    silinecekOzel = ozel;
                                    ozel.friend.id = -1;
                                    ozel.Close();
                                }
                            }
                            myWindow.ozelMesajlasmalar.Remove(silinecekOzel);


                        });
                    }
                    else if (data.Contains("mesajAliciya"))
                    {
                        Console.WriteLine("özel mesaj var");
                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    ozel.lbMesajlar.Items.Add(data.Split('<')[1] + ": " + data.Split('<')[2]);
                                }
                            }

                        });
                    }
                    else if (data.Contains("cikisYapanUyeVar"))
                    {
                        Console.WriteLine("çıııışık yapanlar var");
                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            Uye silinecekUye = null;
                            foreach (Uye item in myWindow.lblClients.Items)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    silinecekUye = item;
                                }
                            }
                            myWindow.lblClients.Items.Remove(silinecekUye);


                        });
                    }
                    else if (data.Contains("yeniOdaBildirimi"))
                    {
                        Console.WriteLine("yeni oda açılmış");
                        sOda yeniOda = new sOda(Convert.ToInt32(data.Split('<')[1]), data.Split('<')[2]);
                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            myWindow.lbOdalar.Items.Add(yeniOda);
                            if( Convert.ToInt32(data.Split('<')[3]) == myWindow.myClient.id)
                            {
                                myWindow.myClient.sendMessage("odayaKatil<" + yeniOda.id);
                                Oda oda = new Oda(yeniOda);
                                oda.lbKatilimcilar.Items.Add("*you*");
                                myWindow.katildigimOdalar.Add(oda);
                                oda.Show();
                            }

                        });
                    }
                    else if (data.Contains("odayaYeniGirenVar"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[2]))
                                {
                                    item.lbKatilimcilar.Items.Add(data.Split('<')[1]);
                                }
                            }

                        });
                    }
                    else if (data.Contains("odaKatilimcilari"))
                    {

                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    foreach(string katilimci in data.Split('<')[2].Split(','))
                                    {
                                        if(katilimci.Length>1)
                                            item.lbKatilimcilar.Items.Add(katilimci);
                                    }
                                  
                                }
                            }

                        });
                    }else if (data.Contains("odaninMesajlariCek"))
                    {
                        
                       
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    item.lbMesajlar.Items.Clear();
                                    string[] mesajlar = data.Split('<')[2].Split('~');
                                    foreach(string mesaj in mesajlar)
                                    {
                                        item.lbMesajlar.Items.Add(mesaj);
                                    }
                                   
                                }
                            }

                        });
                    }else if (data.Contains("odadanBiriCikti"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[2]))
                                {
                                    item.lbKatilimcilar.Items.Remove( "User#"+ data.Split('<')[1]);
                                }
                            }

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
                                        Uye eklenecekUye = new Uye(Convert.ToInt32(uye_bilgileri[0]), uye_bilgileri[1]);
                                     
                                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                                        Application.Current.Dispatcher.Invoke(delegate {
                                            myWindow.lblClients.Items.Add(eklenecekUye);
                                        });
                                    }
                                }
                            }
                            else
                            {
                                string[] odalar = gelen.Split('[');
                                foreach (string oda in odalar)
                                {
                                    if (oda.Contains("<"))
                                    {
                                        string[] oda_bilgileri = oda.Split('<');
                                        sOda eklenecekOda = new sOda(Convert.ToInt32(oda_bilgileri[0]), oda_bilgileri[1]);

                                        Console.WriteLine(eklenecekOda.name + " sisteme eklendi");

                                        Application.Current.Dispatcher.Invoke(delegate {
                                            myWindow.lbOdalar.Items.Add(eklenecekOda);
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
