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
        public List<Client> clientLists = new List<Client>();
        public List<Oda> odalarLists = new List<Oda>();

        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        public Server(bool isLocal, int port)
        {
            string myIP = "127.0.0.1";
            if (!isLocal)
            {
                string hostName = Dns.GetHostName();
                myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            }

            IPAddress localAddr = IPAddress.Parse(myIP);
            server = new TcpListener(localAddr, port);
            
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
            }catch(System.Net.Sockets.SocketException ee)
            {

            }


        }

        public void sendClientMessage(string str, Client client, bool broadcast)
        {
            try
            {
                if (!broadcast)
                {
                    var stream = client.user_tcpclient.GetStream();

                    Byte[] reply = Encoding.UTF32.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                }
                else
                {
                    foreach (Client uye in clientLists)
                    {
                        if (uye != client)
                        {
                            var stream = uye.user_tcpclient.GetStream();

                            Byte[] reply = Encoding.UTF32.GetBytes(str);
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
                    data = Encoding.UTF32.GetString(bytes, 0, i);
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
                                    sendClientMessage("mesajAliciyaEski<" + friend.id + "<" + mesajlar, (Client)obj, false);
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
                            if (uye.nickname == nickname)
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
                        Console.WriteLine("özel mesaj var " + data);
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == alici)
                            {
                                ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj, ((Client)obj).id.ToString());

                                sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + friend.id + ": " + mesaj, friend, false);
                            }
                        }
                    }
                    else if (data.Contains("cikisYapiyorum"))//programdan çıkıldığında
                    {
                        if (((Client)obj).id != -1)
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
                                    item.mesajEkle("SERVER: " + ((Client)obj).nickname + " odaya katildi");
                                    string bulunanlar = "";
                                    foreach (Client uye in item.bulunanlar)
                                    {
                                        sendClientMessage("odayaYeniGirenVar<" + ((Client)obj).id + "<" + item.id + "<" + "SERVER: " + ((Client)obj).nickname + " odaya katildi", uye, false);
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
                                    if (((Client)obj).id != uye.id)
                                        sendClientMessage("odaninMesajlariCek<" + item.id + "<~" + "<" + uyeId + ": " + odaMesaj, uye, false);
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
                   
                    else if (data.Contains("dosyaKabulu"))
                    {
                        string alici = data.Split('<')[1].Split('-')[1];

                        Byte[] bytes1 = File.ReadAllBytes("dosyalar/" + data.Split('<')[1]);
                        String file = Convert.ToBase64String(bytes1);
                        dosyaParcaciklari = Split(file, 65535);//64kb paketlerle gönder
                        dosyaSirasi = 0;

                        sendClientMessage("###dosyaYukleniyor###<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi), (Client)obj, false);
                        //dosya 64kb den az olursa direk gönderip bitirsin
                    }
                    else if (data.Contains("dosyaKontrol"))
                    {
                        clearStream(stream);
                        sendClientMessage("###dosyaYukleniyor###<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi), (Client)obj, false);

                    }

                    else if (data.Contains("###dosyaDevam###"))
                    {
                        if (dosyaParcaciklari != null) {
                            dosyaSirasi++;
                            Console.WriteLine("yüklemeye devam " + dosyaSirasi + "/" + dosyaParcaciklari.Count());
                            clearStream(stream);
                            if (dosyaSirasi < dosyaParcaciklari.Count())
                            {
                                sendClientMessage("###dosyaYukleniyor###<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi), (Client)obj, false);
                            }
                            else
                            {
                                Console.WriteLine("yüklemesi bitti");

                                sendClientMessage("###dosyaBitti###", (Client)obj, false);
                                dosyaSirasi = 0;
                                dosyaParcaciklari = null;


                            }
                        }
                    }

                    else if (data.Contains("###dosyaYukleniyor###"))
                    {
                        if (((Client)obj).gelenDosyaParcaciklari == null)
                        {
                            ((Client)obj).gelenDosyaParcaciklari = new List<string>(); ;
                        }
                        // this.sendMessage("###dosyayiAlmayaBasladim###");
                        if (((Client)obj).gelenDosyaParcaciklari.ElementAtOrDefault(0) != "###DOSYA-GONDERIMI-IPTAL###") {
                            Console.WriteLine("dosya servera ulaştı");

                            string tur = data.Split('<')[1];
                            string alici = data.Split('<')[2];
                            string dosyaAdi = data.Split('<')[3];

                            Application.Current.Dispatcher.Invoke(delegate
                            {
                               
                            /*   if (((ProgressBar)myWindow.fileItem[0]) == null)
                               {
                                   //hata

                               }
                                ((ProgressBar)myWindow.fileItem[0]).Maximum = Convert.ToInt32(data.Split('<')[1].Split('-')[1]);*/
                                int karsiDurum = Convert.ToInt32(data.Split('<')[4].Split('-')[0]);
                                if (karsiDurum == 0 && karsiDurum == ((Client)obj).gelenDosyaParcaciklari.Count)
                                {
                                    Console.WriteLine("ilk paket");
                                    ((Client)obj).gelenDosyaParcaciklari.Add(data.Split('<')[5]);
                                    sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                }
                                else
                                {

                                    Console.WriteLine(karsiDurum + " " + ((Client)obj).gelenDosyaParcaciklari.Count);
                                    if (((Client)obj).gelenDosyaParcaciklari.ElementAtOrDefault(karsiDurum) == data.Split('<')[5])
                                    {//paket doğru
                                     // ((ProgressBar)myWindow.fileItem[0]).Value = Convert.ToInt32(data.Split('<')[1].Split('-')[0]);
                                    sendClientMessage("###dosyaDevam###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                    }
                                    else
                                    {//hatalı veya boşsa
                                    Console.WriteLine("hatalı veya boş paket");
                                        if (karsiDurum < ((Client)obj).gelenDosyaParcaciklari.Count)
                                        {
                                            Console.WriteLine("****************HATALI PAKET TESPİTİ****************");
                                            ((Client)obj).gelenDosyaParcaciklari[karsiDurum] = data.Split('<')[5];
                                            sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                        }
                                        else
                                        {

                                            ((Client)obj).gelenDosyaParcaciklari.Add(data.Split('<')[5]);
                                            sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                        }
                                    }


                                }
                            



                        });
                    }
                    else
                    {
                        ((Client)obj).gelenDosyaParcaciklari = null;
                    }
                    }
                    else if (data.Contains("###dosyaIptal###"))
                    {
                        Console.WriteLine("client dosya gönderimi iptal etti");
                        ((Client)obj).gelenDosyaParcaciklari = new List<string>();
                        ((Client)obj).gelenDosyaParcaciklari.Add("###DOSYA-GONDERIMI-IPTAL###");

                        sendClientMessage("###DOSYA-GONDERIMI-IPTAL###", (Client)obj,false);
                    }
                    else if (data.Contains("###dosyaBitti###"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //son parça geldiğinde
                            string tur = data.Split('<')[1];
                            string alici = data.Split('<')[2];
                            string dosyaAdi = data.Split('<')[3];
                            Byte[] bytes1 = Convert.FromBase64String(string.Join("", ((Client)obj).gelenDosyaParcaciklari));

                            int file_id = 1;
                            while(File.Exists("dosyalar/file-" + alici + "-" + ((Client)obj).id+"-"+file_id))
                            {
                                file_id++;
                            }

                            File.WriteAllBytes("dosyalar/file-" + alici + "-" + ((Client)obj).id + "-" + file_id, bytes1);
                            ((Client)obj).gelenDosyaParcaciklari = new List<string>();

                            string mesaj = "###dosyaVar###dosyaAdi=" + dosyaAdi+"*"+file_id;
                            if (tur == "oda")// room message file send
                            {
                                foreach (Oda item in myWindow.lbOdalar.Items)
                                {
                                    if (item.id == Convert.ToInt32(alici))
                                    {
                                        item.mesajEkle(((Client)obj).id + ": " + mesaj);
                                        foreach (Client uye in item.bulunanlar)
                                        {
                                            if(uye.id != ((Client)obj).id)
                                            sendClientMessage("odaninMesajlariCek<" + item.id + "<~" + "<" + ((Client)obj).id + ": " + mesaj, uye, false);
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
                                        ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj, ((Client)obj).id.ToString());

                                        sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + mesaj , friend, false);
                                    }
                                }
                            }

                   

                        });
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
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
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
                    sw.WriteLine("uye<" + nickname + "<" + id + ">");
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
