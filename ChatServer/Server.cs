using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows; 
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChatServer
{


    /// <summary>
    /// 
    /// </summary>
    class Server
    {
        public TcpListener server = null;
        MainWindow myWindow = null;
        public List<Client> clientLists = new List<Client>();
        public List<Oda> odalarLists = new List<Oda>();
        public Server(bool isLocal, int port, MainWindow cmyWindow)
        {
            string myIP = "127.0.0.1";
            if (!isLocal)
            {
                string hostName = Dns.GetHostName();
                myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            }

            IPAddress localAddr = IPAddress.Parse(myIP);
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

                    sendClientMessage("ConnOK", newUser, false);


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
        public byte[] getData(Client client)
        {
            NetworkStream stream = client.user_tcpclient.GetStream();

            byte[] fileSizeBytes = new byte[4];
            int bytes = stream.Read(fileSizeBytes, 0, 4);
            int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

            int bytesLeft = dataLength;
            byte[] data = new byte[dataLength];

            int bufferSize = 1024;
            int bytesRead = 0;

            while (bytesLeft > 0)
            {
                int curDataSize = Math.Min(bufferSize, bytesLeft);
                if (client.user_tcpclient.Available < curDataSize)
                    curDataSize = client.user_tcpclient.Available; //This saved me

                bytes = stream.Read(data, bytesRead, curDataSize);

                bytesRead += curDataSize;
                bytesLeft -= curDataSize;
            }

            return data;
        }

        public void HandleDeivce(Object obj)
        {

            TcpClient client = (TcpClient)((Client)obj).user_tcpclient;
            var stream = client.GetStream();
            string imei = String.Empty;
            IEnumerable<string> dosyaParcaciklari = null;
            int dosyaSirasi = 0;
            string data = null;
            Byte[] bytes = new Byte[2097152];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    #region gelen komutların değerlendiriliği bölge
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                   // Console.WriteLine("{1}: Received: {0} in Server from " + ((Client)obj).id, data, Thread.CurrentThread.ManagedThreadId);


                    if (data.Contains("sohbetBaslat"))//özel sohbet baslat ve karşı tarafa bildirim gönder
                    {
                        string chatFriend = data.Split('<')[1];
                        Console.WriteLine("sohbet talebi var");
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == chatFriend)
                            {
                                if (ozelMesajVarMi(((Client)obj).id.ToString(), friend.id.ToString()))//sunucuda zaten bir mesaj var ise
                                {
                                    string mesajlar = ozelMesajCek(((Client)obj).id.ToString(), friend.id.ToString());
                                    mesajlar = mesajlar.Replace("###dosyaVar###", "###gonderilmisDosya###");
                                     
                                    sendClientMessage("sohbetTalebiVar<" + ((Client)obj).id + "<" + mesajlar, friend, false);
                                    sendClientMessage("mesajAliciyaEski<" + friend.id+ "<" + mesajlar, (Client)obj, false);
                                }
                                else
                                {
                                    sendClientMessage("sohbetTalebiVar<" + ((Client)obj).id + "<no-message", friend, false);
                                }
                              

                            }
                        }

                    }
                    else if (data.Contains("YeniNickName"))
                    {
                        string nickname = data.Split('<')[1];
                        bool ayniVarmi = false;
                        foreach (Client uye in myWindow.lblClients.Items)
                        {
                            if(uye.nickname == nickname)
                            {
                                ayniVarmi = true;
                            }
                        }
                        if (!ayniVarmi)
                        { 
                            foreach (Client uye in myWindow.lblClients.Items)
                            {//zaten giriş yapmış biri varsa uyarsın
                                if (uye == ((Client)obj))
                                {
                                    string uyeBilgileri = uyeKayitlimi(nickname);
                                    if (uyeBilgileri != null)
                                    {
                                        string[] uyeB = uyeBilgileri.Split('<');
                                        uye.id = Convert.ToInt32(uyeB[2].Replace(">", ""));
                                        uye.nickname = uyeB[1];
                                    }
                                    else
                                    {
                                        uye.id = new Random().Next(1, 999999999);
                                        while (idVarmi(uye.id.ToString()))
                                        {
                                            uye.id = new Random().Next(1, 999999999);
                                        }
                                        uye.nickname = nickname;
                                        idKaydet(uye.id.ToString(), nickname);
                                    }

                                    sendClientMessage("" + uye.id + "~" + uye.nickname + "~" + connectingClient(uye), uye, false);
                                    sendClientMessage("yeniUye=" + uye.id + "<" + uye.nickname, uye, true); //herkese

                                }
                            }
                        }
                        else
                        {
                            sendClientMessage("ayniNickNameVar", (Client)obj, false);
                        }
                       
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lblClients.Items.Refresh();
                        });

                    }

                    else if (data.Contains("mesajVar"))//özel mesaj iletimi
                    {
                        string mesaj = data.Split('<')[1];
                        string alici = data.Split('<')[2];
                        Console.WriteLine("özel mesaj var "+ data);
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == alici)
                            { 
                                ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj, ((Client)obj).id.ToString());

                                sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + friend.id + ": "+mesaj, friend, false);
                            }
                        }
                    } 
                    else if (data.Contains("cikisYapiyorum"))//programdan çıkıldığında
                    {
                        sendClientMessage("cikisYapanUyeVar<" + ((Client)obj).id, (Client)obj, true);//herkese söyle bu arkadaş çıktı
                        clientLists.Remove((Client)obj);
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lblClients.Items.Remove((Client)obj);
                        });
                    }
                    else if (data.Contains("odaOlustur"))//oda oluştur
                    {
                        string odaAdi = data.Split('<')[1];

                        Oda oda = new Oda(odaAdi, (Client)obj);

                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lbOdalar.Items.Add(oda);
                        });
                        odalarLists.Add(oda);
                        sendClientMessage("yeniOdaBildirimi<" + oda.id + "<" + oda.name + "<" + oda.olusturan.id, null, true); // herkese söyle yeni odamız var
                    }
                    else if (data.Contains("odayaKatil"))//odaya katıl
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.lbOdalar.Items)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    item.mesajEkle("SERVER: "+((Client)obj).id + " odaya katildi");
                                    string bulunanlar = "";
                                    foreach (Client uye in item.bulunanlar)
                                    {
                                        sendClientMessage("odayaYeniGirenVar<" + ((Client)obj).id + "<" + item.id + "<" + "SERVER: " + ((Client)obj).id + " odaya katildi", uye, false);
                                        bulunanlar += uye.id + ",";
                                    }
                                    item.bulunanlar.Add((Client)obj);


                                    if (bulunanlar.Length > 1) bulunanlar = bulunanlar.Remove(bulunanlar.Length - 1);
                                    //  sendClientMessage("odaKatilimcilari<" + item.id + "<" + bulunanlar, (Client)obj, false);


                                    sendClientMessage("odaninMesajlariCek<" + item.id + "<" + bulunanlar + "<" + item.mesajTazele().Replace("###dosyaVar###", "###gonderilmisDosya###"), (Client)obj, false); ;
                                }
                            }
                        });
                    }
                    else if (data.Contains("odayaMesajAt"))
                    {
                        int uyeId = ((Client)obj).id;
                        string odaId = data.Split('<')[1];
                        string odaMesaj = data.Split('<')[2];

                        foreach (Oda item in myWindow.lbOdalar.Items)
                        {
                            if (item.id == Convert.ToInt32(odaId))
                            {
                                item.mesajEkle(uyeId + ": " + odaMesaj); 
                                foreach (Client uye in item.bulunanlar)
                                {
                                    sendClientMessage("odaninMesajlariCek<" + item.id +"<~" + "<" +uyeId + ": " + odaMesaj, uye, false);
                                }
                            }
                        }


                    }
                    else if (data.Contains("odadanCikis"))
                    {
                        int uyeId = ((Client)obj).id;
                        string odaId = data.Split('<')[1];
                        foreach (Oda item in myWindow.lbOdalar.Items)
                        {
                            if (item.id == Convert.ToInt32(odaId))
                            {
                                Client silinecekUye = null;
                                foreach (Client uye in item.bulunanlar)
                                {
                                    if (uyeId == uye.id) silinecekUye = uye;
                                }

                                if (silinecekUye != null)
                                {
                                    item.bulunanlar.Remove(silinecekUye);
                                    item.mesajEkle("SERVER: " + silinecekUye.nickname + " is gone");
                                    foreach (Client uye in item.bulunanlar)
                                    {
                                        sendClientMessage("odadanBiriCikti<" + silinecekUye.id + "<" + item.id + "<" + "SERVER: " + silinecekUye.nickname + " is gone", uye, false);
                                    }
                                }
                            }
                        }
                    } 
                    else if (data.Contains("###dosyaVar###"))
                    {
                        string dosyaAdi = data.Split('<')[3];
                        string mesaj = data.Split('<')[2];
                        string alici = data.Split('<')[1];
                        if (data.Contains("OdaIcin"))// room message file send
                        {
                            foreach (Oda item in myWindow.lbOdalar.Items)
                            {
                                if (item.id == Convert.ToInt32(alici))
                                {
                                    item.mesajEkle(((Client)obj).id + ": " + mesaj + dosyaAdi);
                                    foreach (Client uye in item.bulunanlar)
                                    { 
                                        sendClientMessage("odaninMesajlariCek<" + item.id + "<~" + "<" + ((Client)obj).id + ": " + mesaj + dosyaAdi, uye, false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (Client friend in clientLists)// private message file send
                            {
                                if (friend.id.ToString() == alici)
                                {
                                    ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj + dosyaAdi, ((Client)obj).id.ToString());

                                    sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + mesaj + dosyaAdi, friend, false);
                                }
                            }
                        }
                         
                         
                        byte[] fileSizeBytes = new byte[4];
                        int bytes1 = stream.Read(fileSizeBytes, 0, 4);
                        int dataLength = BitConverter.ToInt32(fileSizeBytes, 0);

                        int bytesLeft = dataLength;
                        byte[] data1 = new byte[dataLength];

                        int bufferSize = 1024;
                        int bytesRead = 0;

                        while (bytesLeft > 0)
                        {
                            int curDataSize = Math.Min(bufferSize, bytesLeft);
                            if  ( ((Client)obj) .user_tcpclient.Available < curDataSize)
                                curDataSize = ((Client)obj).user_tcpclient.Available; //This saved me

                            bytes1 = stream.Read(data1, bytesRead, curDataSize);

                            bytesRead += curDataSize;
                            bytesLeft -= curDataSize;
                        }

                        string newFile = "dosyalar/file-"+alici+"-"+ ((Client)obj).id;
                        File.WriteAllBytes(newFile, data1);
                        clearStream(stream);

                    }else if (data.Contains("dosyaKabulu"))
                    {
                        string alici = data.Split('<')[1].Split('-')[1];

                        Byte[] bytes1 = File.ReadAllBytes("dosyalar/" + data.Split('<')[1]);
                        String file = Convert.ToBase64String(bytes1);
                        dosyaParcaciklari = Split(file, 2048);
                        dosyaSirasi = 0;
                       
                        sendClientMessage("###dosyaYukleniyor###<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<"+ dosyaParcaciklari.ElementAt(dosyaSirasi) , (Client)obj, false);
                        
                    }else if (data.Contains("###dosyaDevam###"))
                    {
                        dosyaSirasi++;
                        Console.WriteLine("yüklemeye devam "+dosyaSirasi + "/"+dosyaParcaciklari.Count());
                        clearStream(stream);
                        if (dosyaSirasi < dosyaParcaciklari.Count())
                        { 
                            sendClientMessage("###dosyaYukleniyor###<"+ dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<"  + dosyaParcaciklari.ElementAt(dosyaSirasi), (Client)obj, false);         
                        }
                        else
                        {
                            Console.WriteLine("yüklemesi bitti");
                            sendClientMessage("###dosyaBitti###", (Client)obj, false);
                            dosyaSirasi = 0;
                            dosyaParcaciklari = null;


                        }
                        
                    }

                    #endregion
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        private static void clearStream(NetworkStream stream)
        {
            var buffer = new byte[2097152];
            while (stream.DataAvailable)
            {
                stream.Read(buffer, 0, buffer.Length);
            }
        }
        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        public void addClientToList(Client client)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                myWindow.lblClients.Items.Add(client);
            });
        }

        public string connectingClient(Client me)//bir client bağlandığında serverda bulunan odalar, üyeler fln gönder// oda ismi/id 
        {
            string uyeler = "yeniBaglananlar{";
            foreach (Client client in clientLists)
            {
                if (me != client) uyeler += "[" + client.id + "<" + client.nickname + ""; // 
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
        public void ozelMesajEkle(string p1, string p2, string mesaj, string mesajSahibi)
        {

            string fileName = @"ozeller/ozel-" + p1 + "-" + p2 + ".txt";
            try
            {
                if (File.Exists(@"ozeller/ozel-" + p1 + "-" + p2 + ".txt"))
                {
                    fileName = @"ozeller/ozel-" + p1 + "-" + p2 + ".txt";
                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(mesajSahibi + ": " + mesaj + "~");
                    }
                }
                else if (File.Exists(@"ozeller/ozel-" + p2 + "-" + p1 + ".txt"))
                {
                    fileName = @"ozeller/ozel-" + p2 + "-" + p1 + ".txt";
                    using (StreamWriter sw = File.AppendText(fileName))
                    {
                        sw.WriteLine(mesajSahibi + ": " + mesaj + "~");
                    }
                }
                else
                {
                    // Create a new file     
                    using (FileStream fs = File.Create(fileName))
                    {
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes("Created a Private Room~\n");
                        fs.Write(title, 0, title.Length);

                        Byte[] message = new UTF8Encoding(true).GetBytes(mesajSahibi + ": " + mesaj + "~");
                        fs.Write(message, 0, message.Length);

                    }


                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public string ozelMesajCek(string p1, string p2)
        {
            string fileName = null;
            if (File.Exists(@"ozeller/ozel-" + p1 + "-" + p2 + ".txt"))
            {
                fileName = @"ozeller/ozel-" + p1 + "-" + p2 + ".txt";

            }
            else if (File.Exists(@"ozeller/ozel-" + p2 + "-" + p1 + ".txt"))
            {
                fileName = @"ozeller/ozel-" + p2 + "-" + p1 + ".txt";

            }

            if (fileName != null)
            {
                string sonuc = "";
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";

                    while ((s = sr.ReadLine()) != null)
                    {
                        sonuc += s;
                    }
                }
                return sonuc;
            }
            else
            {
                return "Created a Private Room~";
            }

        }

        public bool ozelMesajVarMi(string p1, string p2)
        {
            if (File.Exists(@"ozeller/ozel-" + p1 + "-" + p2 + ".txt") || (File.Exists(@"ozeller/ozel-" + p2 + "-" + p1 + ".txt")))
            {
                return true;

            }
            return false;

        }

        public bool idKaydet(string id, string nickname)
        {
            string fileName = @"idnumaralari.txt";
            try
            {

                using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
                { 
                    sw.WriteLine("uye<" + nickname + "<" + id + ">\n");
                }
                return true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return false;
            }
        }
        public string uyeKayitlimi(string nickname)
        {
            string fileName = @"idnumaralari.txt";
            if (!File.Exists(@"idnumaralari.txt"))
            {
                return null;
            }


            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("uye<" + nickname + "<"))
                    {
                        return s;
                    }
                }


            }
            return null;

        }
        public bool idVarmi(string id)
        {
            string fileName = @"idnumaralari.txt";
            if (!File.Exists(@"idnumaralari.txt"))
            {
                return false;
            }


            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("<" + id + ">"))
                    {
                        return true;
                    }
                }


            }
            return false;
        }
    }
}
