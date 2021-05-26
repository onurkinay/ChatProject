using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Linq;

namespace ChatClient
{

    // State object for receiving data from remote device.  
    public class Client
    {

        public TcpClient client = null;
        NetworkStream stream = null;
        int deneme = 0;
        MainWindow myWindow = Application.Current.MainWindow as MainWindow;
        ConnectServer cnScreen = null;

        public int id = -1;

        public IEnumerable<string> gidenDosyaParcalari = null;//sunucuya gönderilecek dosya parçaları tutar
        int dosyaSirasi = 0;//sunucuya hangi dosya parçası gönderilecek onu tutar

        public List<string> mesajKuyrugu = new List<string>();

        public Client(ConnectServer connectScreen)
        { 
            cnScreen = connectScreen; 
        }
        public void Connect(String server)
        { 
            try
            {
                int port = 13000;
                client = new TcpClient(server, port);

                stream = client.GetStream();

                int count = 0;
                while (count++ < 3)
                {
                    byte[] data = Encoding.UTF32.GetBytes("connectMe");//sunucuya bağlanma talebi iletir

                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);//ben bağlandım bana serverdan bilgi getir
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        cnScreen.lbStatus.Content = "Bağlandı...";
                    });
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(stream);
                     
                }

            }
            catch (Exception err)
            {//doğru sunucuya bağlanamazsa hata mesajı
                MessageBox.Show("Bağlantı hatası. Lütfen bağlanmak istediğiniz sunucu doğru olduğundan emin olunuz", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("SocketException: {0}", err);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    cnScreen.btnBaglan.IsEnabled = true;
                    cnScreen.lbStatus.Content = "Bağlantı Hatası";
                });
                client = null;

            }

        }

        public void sendData(string safeFileName,string fileName, object type)//sunucuya dosya göndermek için fonksiyon
        { 
             
            Byte[] bytes1 = File.ReadAllBytes(fileName);
            String file = Convert.ToBase64String(bytes1);
            gidenDosyaParcalari = Split(file, 2048);//32kb dosya paketleri
            dosyaSirasi = 0;
            
            myWindow.yukleme.Maximum = gidenDosyaParcalari.Count();
            myWindow.yukleme.Visibility = Visibility.Visible;


            if (type is Uye uye)//özele
            {
                sendMessage("###dosyaYukleniyor###<uye<"+uye.id+"<"+safeFileName+"<"+ dosyaSirasi + "-" + gidenDosyaParcalari.Count());

            }
            else if(type is Oda oda)//odaya
            {
                sendMessage("###dosyaYukleniyor###<oda<" + oda.id + "<"+safeFileName+"<" + dosyaSirasi + "-" + gidenDosyaParcalari.Count());

            }
             

        }

        public static IEnumerable<string> Split(string str, int chunkSize)//gönderilecek dosyaları parçalara ayırır
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        public void HandleDeivce(Object obj)//sunucu-client iletişimi için belirlenen fonksiyon
        { 
            var stream = (NetworkStream)obj;
            string data = null;//gelen veri
            Byte[] bytes = new Byte[1048576];//2mb
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                { 
                    data = Encoding.UTF32.GetString(bytes, 0, i); //utf32 ile türkçe ve emoji karakterleri okuyabiliyor

                    #region genel komutlar
                    if (data.Contains("ConnOK"))// bağlantı sağlandı ve bir nickname al
                    {
                        string[] gelen = data.Split('<');
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.connectServerWindow.btnKabul.IsEnabled = true;
                            myWindow.connectServerWindow.txtNickname.IsEnabled = true;

                            myWindow.connectServerWindow.txtSifre.IsEnabled = true;

                            myWindow.connectServerWindow.cbServer.IsEnabled = false;
                            myWindow.connectServerWindow.btnBaglan.IsEnabled = false;
                             
                        });
                    }
                    else if (data.Contains("yeniBaglananlar"))//kullanıcının belirlediği nickname kaydet ve sunucudan bağlı üyeler ve odaları çek
                    {
                        string[] gelen = data.Split('~');
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.myId = gelen[0];
                            myWindow.myNickName = gelen[1];

                            myWindow.txtId.Text = myWindow.myId;
                            myWindow.Title = "Nickname: " + myWindow.myNickName;
                            myWindow.btnOdaOlustur.IsEnabled = true;
                            myWindow.connectServerWindow.Close();

                            myWindow.btnConnect.Content = "Sunucudan ayrıl";
                        });
                        yeniGelen(gelen[2]);
                    }
                    else if (data.Contains("ayniNickNameVar"))//kullanıcnın nickname zaten başkası kullanıyor
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.connectServerWindow.lbNickName.Content = "Bu nickname kullanılıyor";

                            myWindow.connectServerWindow.btnKabul.IsEnabled = true;
                            myWindow.connectServerWindow.txtNickname.IsEnabled = true;

                            myWindow.connectServerWindow.cbServer.IsEnabled = false;
                            myWindow.connectServerWindow.btnBaglan.IsEnabled = false;

                        });
                    }
                    else if (data.Contains("hataliUyeSifresi"))//kullanıcnın nickname zaten başkası kullanıyor
                    {
                        string[] gelen = data.Split('~');
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.connectServerWindow.lbNickName.Content = "Şifre hatalı.";

                            myWindow.connectServerWindow.btnKabul.IsEnabled = true;
                            myWindow.connectServerWindow.txtNickname.IsEnabled = true;

                            myWindow.connectServerWindow.cbServer.IsEnabled = false;
                            myWindow.connectServerWindow.btnBaglan.IsEnabled = false;

                        });
                    }
                    else if (data.Contains("###serverKapatildi###"))//sunucu kendini kapattı mesajı
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            MessageBox.Show("SUNUCU KAPATILDI. PROGRAM KAPATILACAK", "Sunucu Mesajı", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(2);
                        });
                    }
                   
                    else if (data.Contains("yeniUye="))//sunucuya yeni katılan üyenin bilgileri alır
                    {//yeni katılan kişiyi alır
                        if (myWindow.myId != null)
                        {
                            string gelen = data.Remove(0, 8);//yeniUye=
                            string[] uye_bilgileri = gelen.Split('<');
                            Uye eklenecekUye = new Uye(uye_bilgileri[0], uye_bilgileri[1]);

                            Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                bool eklensinMi = false;
                                if (myWindow.lblClients.Items.Count > 0)
                                {
                                    foreach (Uye uye in myWindow.lblClients.Items)
                                    {
                                        if (uye.id != eklenecekUye.id)
                                            eklensinMi = true;
                                    }
                                    if (eklensinMi) myWindow.lblClients.Items.Add(eklenecekUye);
                                }
                                else
                                {
                                    myWindow.lblClients.Items.Add(eklenecekUye);
                                }

                            });
                        }
                    }
                    else if (data.Contains("cikisYapanUyeVar"))//sunucudan çıkan üyeyi siler
                    {//çıkış yapan üyeyi listeden sil
                        Console.WriteLine("bir üye çıkış yaptı");
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Uye silinecekUye = null;
                            foreach (Uye item in myWindow.lblClients.Items)
                                if (item.id == data.Split('<')[1])
                                    silinecekUye = item;

                            myWindow.lblClients.Items.Remove(silinecekUye);
                        });
                    }
                    #endregion

                    #region özel mesajlaşma bölgesi
                    else if (data.Contains("sohbetTalebiVar"))//özel mesaj atmak isteyen var. Varsa ben penceremi açmadan bilgileri yüklesin
                    {
                        Console.WriteLine(data + " eski mesajlar");

                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Uye skUye = null;
                            Ozel ozel = null;

                            foreach (Uye uye in myWindow.lblClients.Items)
                            {

                                if (uye.id == data.Split('<')[1])
                                {
                                    skUye = uye;
                                    ozel = new Ozel(uye);
                                    ozel.Show();
                                    ozel.Visibility = Visibility.Hidden;
                                    myWindow.ozelMesajlasmalar.Add(ozel);

                                    string mesajlar = data.Split('<')[2];
                                    SifirdanOzelMesajEkle(ozel, mesajlar.Split('~'));

                                    break;
                                }

                            }

                        });

                    }

                    else if (data.Contains("mesajAliciya"))//özel mesajları topluca alır
                    {
                        Console.WriteLine("özel mesaj var");
                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == data.Split('<')[1])
                                {
                                    string mesajlar = data.Split('<')[2];
                                     
                                    Uye skUye = SifirdanOzelMesajEkle(ozel, mesajlar.Split('~'));

                                    if (skUye != null && !(ozel.Visibility == Visibility.Visible))
                                    {//ding dong yeni mesaj var
                                        skUye.DoBlink = true;
                                        myWindow.lblClients.Items.Remove(skUye);
                                        myWindow.lblClients.Items.Insert(0, skUye);
                                    }

                                }
                            }


                        });
                    }

                    else if (data.Contains("mesajTekAliciya"))//karşı tarafın attığı mesajı al
                    {
                        string mesaj = "";
                        if (data.Contains("###dosyaYukleniyor###"))
                        {
                           data =  data.Substring(data.IndexOf("mesajTekAlici") + "mesajTekAlici".Length);
                            if (data.Contains("###dosyaYukleniyor###"))
                                data = data.Remove(data.IndexOf("###dosyaYukleniyor###"));

                            sendMessage("###dosyaKontrol###");//dosya parçası olması gerektiği gibi mi geldi diye sunucudan aynı dosya paketi tekrar iste

                        } 
                        mesaj = data.Split('<')[2];
                        Console.WriteLine("özel mesaj var in client -- " + data);
                      
                        Uye skUye = null;
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == data.Split('<')[1])
                                {
                                    ozel.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(ozel.friend, mesaj.Replace(mesaj.Split(':')[0] + ": ", "")) });
                                    skUye = ozel.friend;

                                }


                                if (skUye != null && !(ozel.Visibility == Visibility.Visible))
                                {//ding dong yeni mesaj var animasyonu
                                    myWindow.PlaySound();
                                    skUye.DoBlink = true;
                                    myWindow.lblClients.Items.Remove(skUye);
                                    myWindow.lblClients.Items.Insert(0, skUye);


                                }

                            }

                        });
                    }
                    #endregion

                    #region oda
                    else if (data.Contains("yeniOdaBildirimi"))
                    {//Yeni oda oluştur, sunucuda bulunan herkesi bildir ve diek odaya katıl
                        Console.WriteLine("yeni oda açılmış");
                        classOda yeniOda = new classOda(Convert.ToInt32(data.Split('<')[1]), data.Split('<')[2]);
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.lbOdalar.Items.Add(yeniOda);
                            if (data.Split('<')[3] == myWindow.myId)
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
                    {//odaya katılan üyeyi listeye ekle
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                                if (item.id == Convert.ToInt32(data.Split('<')[2]))
                                    foreach (Uye uye in myWindow.lblClients.Items)
                                        if (uye.id.ToString() == data.Split('<')[1])
                                        {
                                            item.lbKatilimcilar.Items.Add(uye);

                                            odaMesajEkle(data, item);
                                        }

                        });
                    }
                    else if (data.Contains("odaKatilimcilari"))
                    {//katıldığım odanın katilimcilari çeker
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                    foreach (string katilimci in data.Split('<')[2].Split(','))
                                        foreach (Uye uye in myWindow.lblClients.Items)
                                            if (uye.id.ToString() == katilimci)
                                                item.lbKatilimcilar.Items.Add(uye);

                        });
                    }
                    else if (data.Contains("odaninMesajlariCek"))
                    {//katıldığım odanın önceki mesajları çeker
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[1]))
                                {
                                    odaMesajEkle(data, item);

                                    foreach (string katilimci in data.Split('<')[2].Split(','))
                                        foreach (Uye uye in myWindow.lblClients.Items)
                                            if (uye.id.ToString() == katilimci)
                                                item.lbKatilimcilar.Items.Add(uye);
                                }
                            }

                        });
                    }
                    else if (data.Contains("odadanBiriCikti"))
                    {//odadan çıkan kişiyi siler
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == Convert.ToInt32(data.Split('<')[2]))
                                {
                                    Uye silinecekUye = null;
                                    foreach (Object uye in item.lbKatilimcilar.Items)
                                        if (uye is Uye && ((Uye)uye).id.ToString() == data.Split('<')[1])
                                            silinecekUye = (Uye)uye;

                                    item.lbKatilimcilar.Items.Remove(silinecekUye);
                                    odaMesajEkle(data, item);

                                }
                            }

                        });
                    }
                    else if (data.Contains("buOdaKaldirdim"))
                    {//sunucu oda kapatma bildirisi
                        string odaId = data.Split('<')[1];
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //özel silme kodu
                            classOda silinecekOda = null;
                            foreach (classOda item in myWindow.lbOdalar.Items)
                                if (item.id.ToString() == odaId)
                                    silinecekOda = item;

                            myWindow.lbOdalar.Items.Remove(silinecekOda);

                            foreach (Oda item in myWindow.katildigimOdalar)
                            {
                                if (item.id == silinecekOda.id)
                                {
                                    item.btnGonder.IsEnabled = false;
                                    item.txtMesaj.IsEnabled = false;
                                    item.lbKatilimcilar.Items.Clear();
                                    item.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye("-1", "SERVER"), "ODA KAPATILDI", item) });
                                }
                            }
                        });
                    }
                    #endregion

                    #region dosya işlemleri

                    /////////////////////////DOWNLOAD KISMI/////////////////////////
                    else if (data.Contains("###dosyaYukleniyor###"))
                    { //sunucudan dosya paketleri alır, kontrol eder ve dosyaParcaciklari isimde bir listeye ekler
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            if (mesajKuyrugu.Count != 0)
                            {
                                Console.WriteLine("kuyruktan mesaj alındı");
                                sendMessage(mesajKuyrugu.First() + "<###dosyaKontrol###<-1<-1<-1");//önce mesaj sonra dosya
                                mesajKuyrugu.Remove(mesajKuyrugu.First());

                            }
                            else
                            {
                                if (myWindow.dosyaParcaciklari == null)
                                {
                                    myWindow.dosyaParcaciklari = new List<string>(); ;
                                }
                                if (((ProgressBar)myWindow.fileItem[0]) == null)
                                {
                                }
                             ((ProgressBar)myWindow.fileItem[0]).Maximum = Convert.ToInt32(data.Split('<')[1].Split('-')[1]);
                                int karsiDurum = Convert.ToInt32(data.Split('<')[1].Split('-')[0]);
                                if (karsiDurum == 0 && karsiDurum == myWindow.dosyaParcaciklari.Count)
                                {//ilk paket için özel şart
                                    myWindow.dosyaParcaciklari.Add(data.Split('<')[2]);
                                    sendMessage("###dosyaKontrol###");//dosya parçası olması gerektiği gibi mi geldi diye sunucudan aynı dosya paketi tekrar iste
                                }
                                else
                                {

                                    Console.WriteLine(karsiDurum + " " + myWindow.dosyaParcaciklari.Count);
                                    if (myWindow.dosyaParcaciklari.ElementAtOrDefault(karsiDurum) == data.Split('<')[2])
                                    {//paket doğru
                                        ((ProgressBar)myWindow.fileItem[0]).Value = Convert.ToInt32(data.Split('<')[1].Split('-')[0]);
                                        sendMessage("###dosyaDevam###");//gelen dosya paketi sağlam, sonraki pakete geç
                                    }
                                    else
                                    {

                                        if (karsiDurum < myWindow.dosyaParcaciklari.Count)
                                        {//BİR DOSYA PAKETİ OLMASI GEREKTİĞİ GİBİ GELMEDİ. YERİNE YENİ PAKET İLE DEĞİŞTİR
                                            Console.WriteLine("****************HATALI PAKET TESPİTİ****************");
                                            myWindow.dosyaParcaciklari[karsiDurum] = data.Split('<')[2];
                                            sendMessage("###dosyaKontrol###");
                                        }
                                        else
                                        {
                                            myWindow.dosyaParcaciklari.Add(data.Split('<')[2]);
                                            sendMessage("###dosyaKontrol###");// dosya parçası olması gerektiği gibi mi geldi diye sunucudan aynı dosya paketi tekrar iste


                                        }
                                    }

                                }
                            }
                        });

                    }
                    else if (data.Contains("yenidenGonderim"))
                    { 
                        sendMessage("###dosyaKontrol###");// dosya parçası olması gerektiği gibi mi geldi diye sunucudan aynı dosya paketi tekrar iste

                    }

                    else if (data.Contains("###dosyaBitti###"))
                    {//sunucu son paketi gönderdi ve bilgisayara kaydet
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //son parça geldiğinde
                            Byte[] bytes1 = Convert.FromBase64String(string.Join("", myWindow.dosyaParcaciklari));

                            File.WriteAllBytes(myWindow.saveFilePath, bytes1);//bütün dosya paketleri birleştir ve kaydet
                            myWindow.dosyaParcaciklari = new List<string>();//yeni indirilecek dosya için listeyi sıfırla

                            ((ProgressBar)myWindow.fileItem[0]).Visibility = Visibility.Collapsed;
                            ((Button)myWindow.fileItem[1]).Content = "Dosya indirildi. Tekrar indirmek için tıklayınız";
                            ((Button)myWindow.fileItem[1]).IsEnabled = true;
                            ((Button)myWindow.fileItem[1]).Visibility = Visibility.Visible; 


                        });
                    }
                    else if (data.Contains("###dosyaBulunamadi###"))
                    {//sunucu son paketi gönderdi ve bilgisayara kaydet
                        Application.Current.Dispatcher.Invoke(delegate
                        { 
                            ((ProgressBar)myWindow.fileItem[0]).Visibility = Visibility.Collapsed;
                            ((Button)myWindow.fileItem[1]).Content = "Dosya sunucuda bulunamadı";
                            ((Button)myWindow.fileItem[1]).IsEnabled = false;
                            ((Button)myWindow.fileItem[1]).Visibility = Visibility.Visible;


                        });
                    }

                    /////////////////////////UPLOAD KISMI/////////////////////////
                    ///
                  
                    else if (data.Contains("dosyaKontrol"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            if (gidenDosyaParcalari != null)//--
                            {
                                string tur = data.Split('<')[1];
                                string alici = data.Split('<')[2];
                                string dosyaAdi = data.Split('<')[3];

                                if (mesajKuyrugu.Count != 0)
                                {
                                    Console.WriteLine("kuyruktan mesaj alındı");
                                    sendMessage(mesajKuyrugu.First() + "<###dosyaKontrol###<"+tur+"<"+alici+"<"+dosyaAdi);//önce mesaj sonra dosya
                                    mesajKuyrugu.Remove(mesajKuyrugu.First());
                                }
                                else
                                {
                                    deneme++;
                                    Console.WriteLine(deneme + " dosya kontrolu" + dosyaSirasi + " - " + gidenDosyaParcalari.ElementAt(dosyaSirasi).Substring(0,50));
                                    if (deneme < 3)
                                    {
                                        
                                        sendMessage("###dosyaYukleniyor###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + dosyaSirasi + "-" + gidenDosyaParcalari.Count() + "<" + gidenDosyaParcalari.ElementAt(dosyaSirasi));
                                    }
                                    else
                                    {//tekrara düşerse hafif paket gönder
                                        sendMessage("###yenidenGonderim###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + dosyaSirasi + "-" + gidenDosyaParcalari.Count());
                                        deneme = 0;
                                    }
                                }
                            }
                             
                        });
                    }
                   

                    else if (data.Contains("###dosyaDevam###"))
                    {
                        try
                        {
                       
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                deneme = 0;
                                string tur = data.Split('<')[1];
                                string alici = data.Split('<')[2];
                                string dosyaAdi = data.Split('<')[3];

                                bool odaVarmi = false;
                                if(tur == "oda")
                                {
                                    foreach(Oda oda in myWindow.katildigimOdalar)
                                    {
                                        if(oda.id.ToString() == alici)
                                        {
                                            odaVarmi = true;
                                        }
                                    }

                                    if (!odaVarmi)
                                    {
                                        myWindow.yukleme = null;
                                        gidenDosyaParcalari = null;
                                        dosyaSirasi = 0;
                                        throw new Exception("Dosya yüklerken oda bulunamadı");
                                    }
                                }

                               
                                dosyaSirasi++;
                                myWindow.yukleme.Value = dosyaSirasi;
                                Console.WriteLine("yüklemeye devam " + dosyaSirasi + "/" + gidenDosyaParcalari.Count());
                                clearStream(stream);
                                if (dosyaSirasi < gidenDosyaParcalari.Count())
                                {
                                    sendMessage("###dosyaYukleniyor###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + dosyaSirasi + "-" + gidenDosyaParcalari.Count() + "<" + gidenDosyaParcalari.ElementAt(dosyaSirasi));
                                }
                                else
                                {
                                    Console.WriteLine("yüklemesi bitti");
                                    myWindow.yukleme.Visibility = Visibility.Collapsed;

                                    sendMessage("###dosyaBitti###<" + tur + "<" + alici + "<" + dosyaAdi);
                                    dosyaSirasi = 0;
                                    deneme = 0;
                                    gidenDosyaParcalari = null;
                                    myWindow.yukleme = null;
                                    clearStream(stream);
                                }

                            });
                             
                        }
                        catch
                        {

                        }
                        
                        }
                    #endregion
                }

            }
            catch (IOException e)
            {  
                Console.WriteLine("handledevice: "+e.ToString());
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

        public void sendMessage(string message)
        { 
            Byte[] data = Encoding.UTF32.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void mesajGonder(string message)//dosya indirilmesi veya yüklenmesi esansında mesaj gelirse kuyruğa al
        {
            if (myWindow.yukleme != null || myWindow.dosyaParcaciklari.Count != 0)
            {
                Console.WriteLine("mesaj kuyruğa eklendi");
                mesajKuyrugu.Add(message);
            }
            else
            { 
                sendMessage(message);
            }
        }
        private void odaMesajEkle(string data, Oda item)//odaya mesajları ekler
        {
            Application.Current.Dispatcher.Invoke(delegate
            { 
         
                string[] mesajlar = data.Split('<')[3].Split('~');
                foreach (string mesaj in mesajlar)
                {
                    if (mesaj != "")
                    {
                        if ((mesaj.Split(':')[0] != "SERVER"))
                        {
                            if (mesaj.Contains(":") && myWindow.myId == mesaj.Split(':')[0])
                            {
                                item.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), mesaj.Replace(mesaj.Split(':')[0] + ": ", "").Replace("###dosyaVar###", "###gonderilmisDosya###"), item) });

                            }
                            else
                            {
                                foreach (Uye sUye in myWindow.lblClients.Items)
                                {
                                    if (mesaj.Contains(":") && sUye.id == mesaj.Split(':')[0])
                                    {
                                        item.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(sUye, mesaj.Replace(mesaj.Split(':')[0] + ": ", ""),item) });
                                    }
                                }
                            }
                        }
                        else
                        {
                            item.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(new Uye("-1", "SERVER"), mesaj.Replace(mesaj.Split(':')[0] + ": ", ""),item) });

                        }
                    }
                }
            });
        }

        private Uye SifirdanOzelMesajEkle(Ozel ozel, string[] mesajlar)//özel mesajları topluca girer
        {
            Uye skUye = null;
            Application.Current.Dispatcher.Invoke(delegate
            {
               
                ozel.lbMesajlar.Items.Clear();
                foreach (string mesaj in mesajlar)
                {
                    if (mesaj != "")
                    {
                        if (mesaj.Contains(":") && myWindow.myId == mesaj.Split(':')[0])
                        {
                            ozel.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), mesaj.Replace(mesaj.Split(':')[0] + ": ", "").Replace("###dosyaVar###","###gonderilmisDosya###")) });

                        }
                        else
                        {
                            foreach (Uye sUye in myWindow.lblClients.Items)
                            {
                                if (mesaj.Contains(":") && sUye.id == mesaj.Split(':')[0])
                                {
                                    ozel.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(sUye, mesaj.Replace(mesaj.Split(':')[0] + ": ", "")) });
                                    skUye = sUye;
                                }
                            }
                        }

                    }
                }
            });
            return skUye;
        }

       

        public void yeniGelen(string data)//sunucuya yeni geldim. sunucuda kimler var kimler yok onu görmek istiyorum
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
                                        Uye eklenecekUye = new Uye(uye_bilgileri[0], uye_bilgileri[1]);
                                     
                                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                                        Application.Current.Dispatcher.Invoke(delegate {
                                            bool eklensinMi = false;
                                            if (myWindow.lblClients.Items.Count > 0)
                                            {
                                                foreach (Uye uye in myWindow.lblClients.Items)
                                                {
                                                    if (uye.id != eklenecekUye.id)
                                                        eklensinMi = true;
                                                }
                                                if (eklensinMi) myWindow.lblClients.Items.Add(eklenecekUye);
                                            }
                                            else
                                            {
                                                myWindow.lblClients.Items.Add(eklenecekUye);
                                            }


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
                                        classOda eklenecekOda = new classOda(Convert.ToInt32(oda_bilgileri[0]), oda_bilgileri[1]);

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

            }catch(IOException e)
            {
                Console.WriteLine("bilgileriDegerlendir'den hata "+e.ToString());
            }
            }
    }
    
    
}
