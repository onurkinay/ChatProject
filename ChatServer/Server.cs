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
        public Oda sunucuOdasi = new Oda("Genel Oda", null, 0);
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

            odalarLists.Add(sunucuOdasi);
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
                    
                    sendClientMessage("ConnOK<"+tumKullanicilar(), newUser, false);

                    Console.WriteLine("Connected!");
                    clientLists.Add(newUser);
                   // addClientToList(newUser);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(newUser);
                }
            }catch(SocketException Ex)
            {
                Console.WriteLine("server startlistener hata: "+Ex.ToString());
            }


        }

        public void sendClientMessage(string str, Client client, bool broadcast)//parametreye göre clienta mesaj gönder
        {
            try
            {
                if (!broadcast)//herkese mi yoksa bir clienta mı
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
            catch (Exception Ex)
            {
                Console.WriteLine("Server'den hata "+Ex.ToString());
            }
        }
       
        public void HandleDeivce(Object obj)
        {

            TcpClient client = ((Client)obj).user_tcpclient;
            var stream = client.GetStream();
            string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[1048576];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                  
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.UTF32.GetString(bytes, 0, i);

                    #region uye işlemleri
                    if (data.Contains("sohbetBaslat"))//özel sohbet baslat 
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


                                    sendClientMessage("sohbetTalebiVar<" + ((Client)obj).id + "<" + mesajlar, friend, false);
                                    sendClientMessage("mesajAliciyaEski<" + friend.id + "<" + mesajlar, (Client)obj, false);
                                }
                                else
                                {//sunucuda mesajı yoksa no-message göndersin
                                    sendClientMessage("sohbetTalebiVar<" + ((Client)obj).id + "<no-message", friend, false);
                                }


                            }
                        }

                    }
                    else if (data.Contains("YeniNickName"))//clientın nickname tanımlanması
                    {
                        string nickname = data.Split('<')[1];
                        string sifre = data.Split('<')[2];
                        bool ayniVarmi = false;
                        foreach (Client uye in clientLists)  //şuan sunucuda aynı nickname olan var mı
                            if (uye.nickname == nickname)
                                ayniVarmi = true;

                        bool kaydet = false;
                        if (!ayniVarmi)//aynı nickname değilse
                        {
                            foreach (Client uye in clientLists)
                            {
                                if (uye == ((Client)obj))
                                {
                                    string uyeBilgileri = uyeKayitlimi(nickname);//nickname sunucuda kayıtlı mı
                                    if (uyeBilgileri != null)
                                    {

                                        //uye<nickname<sifre<id>
                                        string[] uyeB = uyeBilgileri.Split('<');
                                        if (sifre == uyeB[2])
                                        {
                                            uye.id = Convert.ToInt32(uyeB[3].Replace(">", ""));
                                            uye.nickname = uyeB[1];
                                            uye.sifre = uyeB[2];
                                        }
                                        else
                                        {
                                            sendClientMessage("hataliUyeSifresi", (Client)obj, false);//şifre hatalı
                                            kaydet = true;

                                        }
                                    }
                                    else
                                    {
                                        //ilk giriş
                                        uye.id = new Random().Next(1, 999999999);
                                        while (idVarmi(uye.id.ToString()))//id çakışması önleme
                                        {
                                            uye.id = new Random().Next(1, 999999999);
                                        }
                                        uye.nickname = nickname;
                                        uye.sifre = sifre;
                                        idKaydet(uye.id.ToString(), nickname, sifre);
                                    }
                                    if (!kaydet)
                                    {
                                        addClientToList(uye);
                                        // odalarLists[0].bulunanlar.Add(uye);
                                        sendClientMessage("" + uye.id + "~" + uye.nickname + "~" + connectingClient(uye), uye, false);//yeni nickname onaylandı ve atanan idsi clienta gönderildi
                                        sendClientMessage("yeniUye=" + uye.id + "<" + uye.nickname, uye, true); //herkese yeni katılanımız olduğunu söyle
                                    }

                                }
                            }
                        }
                        else
                        {
                            sendClientMessage("ayniNickNameVar", (Client)obj, false);//bu nickname sunucuda olduğunu bildir
                        }



                    }
                    else if (data.Contains("cikisYapiyorum"))//programdan çıkıldığında
                    {
                        if (((Client)obj).id != -1)
                            sendClientMessage("cikisYapanUyeVar<" + ((Client)obj).id, (Client)obj, true);//herkese söyle bu arkadaş çıktı
 
                        sunucuOdasi.mesajEkle("SERVER: " + ((Client)obj).nickname + " sunucudan ayrıldı");
                         

                        clientLists.Remove((Client)obj);//sunucudan kaldır
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lblClients.Items.Remove((Client)obj);//sunucu ekran listesinden kaldır
                        });
                    }
                    #endregion

                    #region özel mesajlar için
                    else if (data.Contains("mesajVar"))//özel mesaj iletimi
                    {
                        string mesaj = data.Split('<')[1];
                        string alici = data.Split('<')[2];
                        Console.WriteLine("özel mesaj var " + data);
                        foreach (Client friend in clientLists)
                        {
                            if (friend.id.ToString() == alici)
                            { 
                                //mesaji ilgili text dosyasına kaydet
                                ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj, ((Client)obj).id.ToString());

                                //gelen mesajı alıcı clienta ilet
                                sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + friend.id + ": " + mesaj, friend, false);
                            }
                        }

                        if (data.Contains("dosyaKontrol"))//mesaj dosya yukleme veya indirme esnasında geldiyse
                        {

                            string tur = data.Split('<')[4];
                            string alici2 = data.Split('<')[5];
                            string dosyaAdi = data.Split('<')[6];
                            if (tur == "-1" && alici2 == "-1" && dosyaAdi == "-1")
                            {
                                sendClientMessage("###dosyaYukleniyor###<" + ((Client)obj).dosyaSirasi + "-" + ((Client)obj).dosyaParcaciklari.Count() + "<" + ((Client)obj).dosyaParcaciklari.ElementAt(((Client)obj).dosyaSirasi), (Client)obj, false);
                            }
                            else
                            {
                                sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici2 + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                            }
                        }
                    }
                    #endregion

                    #region oda işlemleri
                    else if (data.Contains("odaOlustur"))//oda oluştur
                    {
                        string odaAdi = data.Split('<')[1];
                        Oda oda = new Oda(odaAdi, (Client)obj, new Random().Next(1, 9999999));

                        //odayı sunucuya kaydet
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lbOdalar.Items.Add(oda);
                        });
                        odalarLists.Add(oda);

                        // herkese söyle yeni odamız var
                        sendClientMessage("yeniOdaBildirimi<" + oda.id + "<" + oda.name + "<" + oda.olusturan.id, null, true);
                    }
                    else if (data.Contains("odayaKatil"))//client odaya katılmak istiyor
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //odayı bulur
                            foreach (Oda item in odalarLists)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {

                                    item.mesajEkle("SERVER: " + ((Client)obj).nickname + ((item.id!=0) ? " odaya katildi": " sunucuya katildi"));//server mesajı
                                    string bulunanlar = "";
                                    foreach (Client uye in item.bulunanlar)
                                    {//odada bulunanlara söyle aramıza biri katıldı
                                        sendClientMessage("odayaYeniGirenVar<" + ((Client)obj).id + "<" + item.id + "<" + "SERVER: " + ((Client)obj).nickname + ((item.id != 0) ? " odaya katildi" : " sunucuya katildi"), uye, false);
                                        bulunanlar += uye.id + ",";
                                    }
                                    item.bulunanlar.Add((Client)obj);


                                    if (bulunanlar.Length > 1) bulunanlar = bulunanlar.Remove(bulunanlar.Length - 1);
                                    //yeni katılana odada bulunanları söyler
                                    sendClientMessage("odaninMesajlariCek<" + item.id + "<" + bulunanlar + "<" + item.mesajTazele(), (Client)obj, false); ;
                                }
                            }
                        });
                    }
                    else if (data.Contains("odayaMesajAt"))//bir client odaya mesaj attı
                    {
                        int uyeId = ((Client)obj).id;
                        string odaId = data.Split('<')[1];
                        string odaMesaj = data.Split('<')[2];

                        foreach (Oda item in odalarLists)
                        {
                            if (item.id == Convert.ToInt32(odaId))
                            {
                                item.mesajEkle(uyeId + ": " + odaMesaj);//mesajı text olarak kaydet
                                foreach (Client uye in item.bulunanlar)
                                {
                                    if (((Client)obj).id != uye.id)
                                        sendClientMessage("odaninMesajlariCek<" + item.id + "<~" + "<" + uyeId + ": " + odaMesaj, uye, false);
                                }
                            }
                        }

                        if (data.Contains("dosyaKontrol"))//mesaj dosya kontrolunden geldiyse
                        {
                            string tur = data.Split('<')[4];
                            string alici2 = data.Split('<')[5];
                            string dosyaAdi = data.Split('<')[6];
                            sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici2 + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);

                        }


                    }
                    else if (data.Contains("odadanCikis")) //client odadan çıktığını söylüyor
                    {
                        int uyeId = ((Client)obj).id;
                        string odaId = data.Split('<')[1];

                        foreach (Oda item in odalarLists)
                        {
                            if (item.id == Convert.ToInt32(odaId))
                            {
                                Client silinecekUye = null;
                                foreach (Client uye in item.bulunanlar)
                                    if (uyeId == uye.id) silinecekUye = uye;//silinecek client bilgilerine ulaşma


                                if (silinecekUye != null)//silinecek client bilgileri varsa
                                {
                                    item.bulunanlar.Remove(silinecekUye);
                                    item.mesajEkle("SERVER: " + silinecekUye.nickname + " odadan ayrıldı");
                                    foreach (Client uye in item.bulunanlar)
                                    {
                                        sendClientMessage("odadanBiriCikti<" + silinecekUye.id + "<" + item.id + "<" + "SERVER: " + silinecekUye.nickname + " odadan ayrıldı", uye, false);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region clienta dosya aktarımı
                    else if (data.Contains("dosyaKabulu"))//client kendisine gönderilen dosyayı kabul ederse
                    {
                        if (File.Exists("dosyalar/" + data.Split('<')[1]))//dosya sunucuda var mı
                        {
                            string alici = data.Split('<')[1].Split('-')[1];//alıcı id

                            Byte[] bytes1 = File.ReadAllBytes("dosyalar/" + data.Split('<')[1]);//dosyayı byte array yap
                            String file = Convert.ToBase64String(bytes1);//dosyanın byte arrayı, base64 stringe çevir
                            ((Client)obj).dosyaParcaciklari = Split(file, 2048);//base64 stringi, 10kb olacak şekilde böl
                            ((Client)obj).dosyaSirasi = 0;

                            //dosya gönderimi başlat
                            sendClientMessage("###dosyaYukleniyor###<" + ((Client)obj).dosyaSirasi + "-" + ((Client)obj).dosyaParcaciklari.Count() + "<" + ((Client)obj).dosyaParcaciklari.ElementAt(((Client)obj).dosyaSirasi), (Client)obj, false);

                        }
                        else
                        {//dosya sunucuda bulunamadıysa 
                            sendClientMessage("###dosyaBulunamadi###<", (Client)obj, false);

                        }
                    }
                    else if (data.Contains("dosyaKontrol"))//dosya parçası olması gerektiği gibi iletilmiş mi bir kontrol et
                    {
                        ((Client)obj).deneme++;

                        if (((Client)obj).deneme < 3)
                        {
                            clearStream(stream);
                            sendClientMessage("###dosyaYukleniyor###<" + ((Client)obj).dosyaSirasi + "-" + ((Client)obj).dosyaParcaciklari.Count() + "<" + ((Client)obj).dosyaParcaciklari.ElementAt(((Client)obj).dosyaSirasi), (Client)obj, false);
                        }
                        else
                        {
                            sendClientMessage("###yenidenGonderim###", (Client)obj, false);
                            ((Client)obj).deneme = 0;
                        }
                    }

                    else if (data.Contains("###dosyaDevam###"))
                    {//dosya parcası iyi iletildiyse sonraki parçaya geç
                        if (((Client)obj).dosyaParcaciklari != null)
                        {
                            ((Client)obj).dosyaSirasi++;
                            Console.WriteLine("yüklemeye devam " + ((Client)obj).dosyaSirasi + "/" + ((Client)obj).dosyaParcaciklari.Count());
                            clearStream(stream);
                            if (((Client)obj).dosyaSirasi < ((Client)obj).dosyaParcaciklari.Count())
                            {
                                sendClientMessage("###dosyaYukleniyor###<" + ((Client)obj).dosyaSirasi + "-" + ((Client)obj).dosyaParcaciklari.Count() + "<" + ((Client)obj).dosyaParcaciklari.ElementAt(((Client)obj).dosyaSirasi), (Client)obj, false);
                            }
                            else
                            {
                                Console.WriteLine("yüklemesi bitti");

                                sendClientMessage("###dosyaBitti###", (Client)obj, false);
                                ((Client)obj).dosyaSirasi = 0;
                                ((Client)obj).dosyaParcaciklari = null;


                            }
                        }
                    }

                    #endregion

                    #region clienttan dosya alımı
                    else if (data.Contains("###yenidenGonderim###"))
                    {//tekrara düşmesini önlemek için
                        string tur = data.Split('<')[1];
                        string alici = data.Split('<')[2];
                        string dosyaAdi = data.Split('<')[3];

                        sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);


                    }
                    else if (data.Contains("###dosyaYukleniyor###"))
                    {//gelen dosya paketini al ve listeye ekle
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            if (((Client)obj).gelenDosyaParcaciklari == null)
                                ((Client)obj).gelenDosyaParcaciklari = new List<string>(); ;


                            string tur = data.Split('<')[1];
                            string alici = data.Split('<')[2];
                            string dosyaAdi = data.Split('<')[3];
                            if (data.Split('<').Length == 5)
                            {
                                sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);

                            }
                            else
                            {

                                int karsiDurum = Convert.ToInt32(data.Split('<')[4].Split('-')[0]);
                                if (karsiDurum == 0 && karsiDurum == ((Client)obj).gelenDosyaParcaciklari.Count)
                                {
                                    Console.WriteLine("ilk paket");
                                    ((Client)obj).gelenDosyaParcaciklari.Add(data.Split('<')[5]);
                                    sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                }
                                else
                                {

                                    Console.WriteLine("Dosya alımı: "+data.Split('<')[4] + " "+dosyaAdi);
                                    if (((Client)obj).gelenDosyaParcaciklari.ElementAtOrDefault(karsiDurum) == data.Split('<')[5])
                                    {//paket doğru
                                        sendClientMessage("###dosyaDevam###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                    }
                                    else
                                    {//hatalı veya boşsa
                                     
                                        if (karsiDurum < ((Client)obj).gelenDosyaParcaciklari.Count)
                                        {
                                            Console.WriteLine("****************HATALI PAKET TESPİTİ****************");
                                            ((Client)obj).gelenDosyaParcaciklari[karsiDurum] = data.Split('<')[5];
                                            sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                        }
                                        else
                                        {
                                            Console.WriteLine("paket alındı");
                                            ((Client)obj).gelenDosyaParcaciklari.Add(data.Split('<')[5]);
                                            sendClientMessage("###dosyaKontrol###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + data.Split('<')[4], (Client)obj, false);
                                        }
                                    }

                                }
                            }
                        });
                    } 
                    else if (data.Contains("###dosyaBitti###"))//dosya paket gönderimi bitti ve ilgili alıcıya mesaj olarak ilet
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //son parça geldiğinde
                            string tur = data.Split('<')[1];
                            string alici = data.Split('<')[2];
                            string dosyaAdi = data.Split('<')[3];

                            //gelen dosya parçaları birleştir ve byte arraye çevir
                            Byte[] bytes1 = Convert.FromBase64String(string.Join("", ((Client)obj).gelenDosyaParcaciklari));

                          
                            int file_id = 1; //aynı isimle olan dosyanın üzerine yazmasın diye id kullanarak kaydedilecektir
                            while (File.Exists("dosyalar/file-" + alici + "-" + ((Client)obj).id + "-" + file_id))
                            {//aynı dosya varmı kontrol et varsa bir artır tekrar kontrol et
                                file_id++;
                            }

                            //dosyayı sunucuya kaydet
                            File.WriteAllBytes("dosyalar/file-" + alici + "-" + ((Client)obj).id + "-" + file_id, bytes1);
                            ((Client)obj).gelenDosyaParcaciklari = new List<string>();


                            //gelen dosyayı ilgili alıcıya ilet
                            string mesaj = "###dosyaVar###dosyaAdi=" + dosyaAdi + "*" + file_id;
                            if (tur == "oda")// odaya gönder
                            {
                                foreach (Oda item in odalarLists)
                                {
                                    if (item.id == Convert.ToInt32(alici))
                                    {
                                        item.mesajEkle(((Client)obj).id + ": " + mesaj);
                                        foreach (Client uye in item.bulunanlar)
                                        {
                                            if (uye.id != ((Client)obj).id)
                                                sendClientMessage("odaninMesajlariCek<" + item.id + "<~" + "<" + ((Client)obj).id + ": " + mesaj, uye, false);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (Client friend in clientLists)// özel mesaja gönder
                                {
                                    if (friend.id.ToString() == alici)
                                    {
                                        ozelMesajEkle(alici, ((Client)obj).id.ToString(), mesaj, ((Client)obj).id.ToString());

                                        sendClientMessage("mesajTekAliciya<" + ((Client)obj).id + "<" + mesaj, friend, false);
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
             
            }
        }

        private static void clearStream(NetworkStream stream)//stream temizleme
        {
            var buffer = new byte[1048576];
            while (stream.DataAvailable)
            {
                stream.Read(buffer, 0, buffer.Length);
            }
        }
        static IEnumerable<string> Split(string str, int chunkSize)//dosyayı belirlenen uzunluklara böl
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }
        public void addClientToList(Client client)//clienti listeye ekle
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                myWindow.lblClients.Items.Add(client);
            });
        }

        public string connectingClient(Client me)//bir client bağlandığında serverda bulunan odalar ve clientları tek string haline çevirir
        {
            string uyeler = "yeniBaglananlar{";
            foreach (Client client in myWindow.lblClients.Items)
            {
                if (me != client) uyeler += "[" + client.id + "<" + client.nickname + ""; //sunucuda bulunan bütün clientlar
            }
            uyeler += "}";

            string odalar = "{";
            foreach (Oda oda in odalarLists)
            {
                if (oda.id == 0) continue;
                odalar += "[" + oda.id + "<" + oda.name + ""; //  sunucuda bulunan bütün odalar
            }
            odalar += "}";

            return uyeler + odalar;
        }
        public void ozelMesajEkle(string p1, string p2, string mesaj, string mesajSahibi)//özel mesajı ilgili texte kaydeder
        {
            //text adı formatı: ozeller/ozel-(client1.id)-(client2.id).txt
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
                    
                    using (FileStream fs = File.Create(fileName))
                    {
                       
                        Byte[] title = new UTF8Encoding(true).GetBytes("Özel oda başlatıldı~\n");
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

        public string ozelMesajCek(string p1, string p2)//ilgili iki clientin konuşmaları döner 
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
            {//ilgili dosya bulunamadıysa
                return "Created a Private Room~";
            }

        }

        public bool ozelMesajVarMi(string p1, string p2)
        {//ilgili iki client arasında önceden bir konuşma oldu mu
            if (File.Exists(@"ozeller/ozel-" + p1 + "-" + p2 + ".txt") || (File.Exists(@"ozeller/ozel-" + p2 + "-" + p1 + ".txt")))
                return true;

           
            return false;

        }

        public bool idKaydet(string id, string nickname, string sifre)//clienta atanan id, nickname ile birlikte kaydeder
        {
            string fileName = @"idnumaralari.txt";
            try
            {

                using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
                {
                    sw.WriteLine("uye<" + nickname + "<" + sifre + "<" + id + ">");

                }
                return true;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return false;
            }
        }
        public string uyeKayitlimi(string nickname)//nicknamein kaydı var mı, varsa
        {
            string fileName = @"idnumaralari.txt";
            if (!File.Exists(@"idnumaralari.txt")) //id numaların saklandığı text dosyası var mı
                return null;
          
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
        public bool idVarmi(string id)//id numarası başka nickname ile eşleşmiş mi
        {
            string fileName = @"idnumaralari.txt";
            if (!File.Exists(@"idnumaralari.txt")) 
                return false;
      


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

        public string tumKullanicilar()//id numarası başka nickname ile eşleşmiş mi
        {
            string fileName = @"idnumaralari.txt";
            string result = "";
            if (!File.Exists(@"idnumaralari.txt"))
                return null;

            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    string[] bilgiler= s.Split('<');
                    result += bilgiler[0]+"<"+ bilgiler[1]+"<"+ bilgiler[3].Replace(">","") +"~";//bypass şifre
                }
                if(result != "") result = result.Substring(0, result.Length - 1);

            }
            return result;
        }
    }
}
