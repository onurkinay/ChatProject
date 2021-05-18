using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Collections;

namespace ChatClient
{

    // State object for receiving data from remote device.  
    public class Client
    {
        public TcpClient client = null;
        NetworkStream stream = null;

        MainWindow myWindow = null;
        ConnectServer cnScreen = null;

        public int id = -1;

        IEnumerable<string> dosyaParcaciklari = null;
        int dosyaSirasi = 0;

        public Client(MainWindow cmyWindow, ConnectServer connectScreen)
        {
            myWindow = cmyWindow;
            cnScreen = connectScreen;
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
                    Byte[] data = System.Text.Encoding.UTF8.GetBytes("connectMe");

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
            {
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

        public void sendData(string safeFileName,string fileName, object type)
        { 
             
            Byte[] bytes1 = File.ReadAllBytes(fileName);
            String file = Convert.ToBase64String(bytes1);
            dosyaParcaciklari = Split(file, 65535);//64kb
            dosyaSirasi = 0;
            
            myWindow.yukleme.Maximum = dosyaParcaciklari.Count();
            myWindow.yukleme.Visibility = Visibility.Visible;


            if (type is Uye uye)
            {
                sendMessage("###dosyaYukleniyor###<uye<"+uye.id+"<"+safeFileName+"<"+ dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi));

            }
            else if(type is Oda oda)
            {
                sendMessage("###dosyaYukleniyor###<oda<" + oda.id + "<"+safeFileName+"<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi));

            }



        }

        static IEnumerable<string> Split(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, Math.Min(chunkSize, str.Length - i));
        }

        public void HandleDeivce(Object obj)
        {

            
            var stream = (NetworkStream)obj;
            string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[2097152];//2mb
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.UTF8.GetString(bytes, 0, i);
               
                    #region genel komutlar
                    if (data.Contains("yeniBaglananlar"))
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

                            myWindow.btnConnect.IsEnabled = false;
                        });
                        yeniGelen(gelen[2]);
                    }
                    else if (data.Contains("ConnOK"))
                    {
                        string[] gelen = data.Split('<');
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            myWindow.connectServerWindow.btnKabul.IsEnabled = true;
                            myWindow.connectServerWindow.txtNickname.IsEnabled = true;

                            myWindow.connectServerWindow.cbServer.IsEnabled = false;
                            myWindow.connectServerWindow.btnBaglan.IsEnabled = false;


                        });
                    }
                    else if (data.Contains("###serverKapatildi###"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            MessageBox.Show("SUNUCU KAPATILDI. PROGRAM KAPATILACAK", "Sunucu Mesajı", MessageBoxButton.OK, MessageBoxImage.Error);
                            Environment.Exit(2);
                        });
                    }
                    else if (data.Contains("ayniNickNameVar"))
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
                    else if (data.Contains("yeniUye="))
                    {//yeni katılan kişiyi alır
                        string gelen = data.Remove(0, 8);//yeniUye=
                        string[] uye_bilgileri = gelen.Split('<');
                        Uye eklenecekUye = new Uye(uye_bilgileri[0], uye_bilgileri[1]);

                        Console.WriteLine(eklenecekUye.nickname + " sisteme eklendi");

                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            myWindow.lblClients.Items.Add(eklenecekUye);

                        });

                    }
                    else if (data.Contains("cikisYapanUyeVar"))
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
                    else if (data.Contains("sohbetTalebiVar"))
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

                                    string mesajlar = data.Split('<')[2].Replace("###dosyaVar###", "###gonderilmisDosya###");
                                    SifirdanOzelMesajEkle(ozel, mesajlar.Split('~'));

                                    break;
                                }

                            }

                        });

                    }

                    else if (data.Contains("mesajAliciya"))
                    {//karşı tarafın attığı mesajı al
                        Console.WriteLine("özel mesaj var");
                        Application.Current.Dispatcher.Invoke(delegate
                        {

                            foreach (Ozel ozel in myWindow.ozelMesajlasmalar)
                            {
                                if (ozel.friend.id == data.Split('<')[1])
                                {
                                    string mesajlar = data.Split('<')[2];

                                    if (data.Contains("mesajAliciyaEski"))
                                        mesajlar = mesajlar.Replace("###dosyaVar###", "###gonderilmisDosya###");
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

                    else if (data.Contains("mesajTekAliciya"))
                    {//karşı tarafın attığı mesajı al
                        Console.WriteLine("özel mesaj var in client -- " + data);
                        string mesaj = data.Split('<')[2];
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
                                {//ding dong yeni mesaj var
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
                    {
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
                    {
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
                    {
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
                    {
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
                    else if (data.Contains("###dosyaYukleniyor###"))
                    {
                        // this.sendMessage("###dosyayiAlmayaBasladim###");
                     
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            if (myWindow.dosyaParcaciklari == null)
                            {
                                myWindow.dosyaParcaciklari = new List<string>(); ;
                            }
                            if (((ProgressBar)myWindow.fileItem[0]) == null)
                            {
                                //hata

                            }
                             ((ProgressBar)myWindow.fileItem[0]).Maximum = Convert.ToInt32(data.Split('<')[1].Split('-')[1]);
                            int karsiDurum = Convert.ToInt32(data.Split('<')[1].Split('-')[0]);
                            if (karsiDurum == 0 && karsiDurum == myWindow.dosyaParcaciklari.Count)
                            {
                                myWindow.dosyaParcaciklari.Add(data.Split('<')[2]);
                                sendMessage("###dosyaKontrol###");
                            }
                            else
                            {

                                Console.WriteLine(karsiDurum + " " + myWindow.dosyaParcaciklari.Count);
                                if (myWindow.dosyaParcaciklari.ElementAtOrDefault(karsiDurum) == data.Split('<')[2])
                                {//paket doğru
                                    ((ProgressBar)myWindow.fileItem[0]).Value = Convert.ToInt32(data.Split('<')[1].Split('-')[0]);
                                    sendMessage("###dosyaDevam###");
                                }
                                else
                                {//hatalı veya boşsa

                                    if (karsiDurum < myWindow.dosyaParcaciklari.Count)
                                    {
                                        Console.WriteLine("****************HATALI PAKET TESPİTİ****************");
                                        myWindow.dosyaParcaciklari[karsiDurum] = data.Split('<')[2];
                                        sendMessage("###dosyaKontrol###");
                                    }
                                    else
                                    {

                                        myWindow.dosyaParcaciklari.Add(data.Split('<')[2]);
                                        sendMessage("###dosyaKontrol###");
                                    }
                                }


                            }



                        });

                    }
                    else if (data.Contains("###dosyaBitti###"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            //son parça geldiğinde
                            Byte[] bytes1 = Convert.FromBase64String(string.Join("", myWindow.dosyaParcaciklari));

                            File.WriteAllBytes(myWindow.saveFilePath, bytes1);
                            myWindow.dosyaParcaciklari = new List<string>();
                            ((ProgressBar)myWindow.fileItem[0]).Visibility = Visibility.Collapsed;
                            ((Button)myWindow.fileItem[1]).Content = "Dosya indirildi";
                            ((Button)myWindow.fileItem[1]).IsEnabled = false;
                            ((Button)myWindow.fileItem[1]).Visibility = Visibility.Visible;


                        });
                    }


                    else if (data.Contains("dosyaKontrol"))
                    {
                        if (dosyaParcaciklari.ElementAt(0) != "###DOSYA-GONDERIMI-IPTAL###")
                        {
                            Console.WriteLine("dosya kontrolu");
                            string tur = data.Split('<')[1];
                            string alici = data.Split('<')[2];
                            string dosyaAdi = data.Split('<')[3];
                            clearStream(stream);
                            sendMessage("###dosyaYukleniyor###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi));
                        }
                    }
                    else if (data.Contains("###DOSYA-GONDERIMI-IPTAL###"))
                    {
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            Console.WriteLine("yüklemesi iptal edildi");
                            dosyaSirasi = 0;
                            myWindow.yukleme = null;
                            //  myWindow.yukleme = null;
                            clearStream(stream);
                        });
                    }

                    else if (data.Contains("###dosyaDevam###"))
                    {
                        try {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                if (dosyaParcaciklari.ElementAt(0) != "###DOSYA-GONDERIMI-IPTAL###")
                                {
                                    string tur = data.Split('<')[1];
                                    string alici = data.Split('<')[2];
                                    string dosyaAdi = data.Split('<')[3];
                                    dosyaSirasi++;
                                    myWindow.yukleme.Value = dosyaSirasi;
                                    Console.WriteLine("yüklemeye devam " + dosyaSirasi + "/" + dosyaParcaciklari.Count());
                                    clearStream(stream);
                                    if (dosyaSirasi < dosyaParcaciklari.Count())
                                    {
                                        sendMessage("###dosyaYukleniyor###<" + tur + "<" + alici + "<" + dosyaAdi + "<" + dosyaSirasi + "-" + dosyaParcaciklari.Count() + "<" + dosyaParcaciklari.ElementAt(dosyaSirasi));
                                    }
                                    else
                                    {
                                        Console.WriteLine("yüklemesi bitti");
                                        myWindow.yukleme.Visibility = Visibility.Collapsed;

                                        Button buton = ((Button)myWindow.yukleme.Tag);
                                        buton.Content = "Dosya yüklendi";
                                        buton.IsEnabled = false;
                                        Grid.SetColumn(buton, 0);
                                        Grid.SetColumnSpan(buton, 2);

                                        sendMessage("###dosyaBitti###<" + tur + "<" + alici + "<" + dosyaAdi);
                                        dosyaSirasi = 0;
                                        dosyaParcaciklari = null;
                                        myWindow.yukleme = null;


                                    }
                                }
                                else
                                {
                                    Console.WriteLine("yüklemesi iptal edildi");
                                    myWindow.yukleme.Visibility = Visibility.Collapsed;

                                    sendMessage("###dosyaIptal###");
                                    dosyaSirasi = 0;
                                    myWindow.yukleme = null;

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

        public void sendMessage(string message)
        {
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        private void odaMesajEkle(string data, Oda item)
        {
            Application.Current.Dispatcher.Invoke(delegate
            { 
               // item.lbMesajlar.Items.Clear();
                string[] mesajlar = data.Split('<')[3].Split('~');
                foreach (string mesaj in mesajlar)
                {
                    if (mesaj != "")
                    {
                        if ((mesaj.Split(':')[0] != "SERVER"))
                        {
                            if (mesaj.Contains(":") && myWindow.myId == mesaj.Split(':')[0])
                            {
                                item.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), mesaj.Replace(mesaj.Split(':')[0] + ": ", ""), item) });

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

        private Uye SifirdanOzelMesajEkle(Ozel ozel, string[] mesajlar)
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
                            ozel.lbMesajlar.Items.Add(new ListBoxItem { Content = new Message(myWindow.getMyUye(), mesaj.Replace(mesaj.Split(':')[0] + ": ", "")) });

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
                                        Uye eklenecekUye = new Uye(uye_bilgileri[0], uye_bilgileri[1]);
                                     
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

            }catch(System.IO.IOException e)
            {
                Console.WriteLine("bilgileriDegerlendir'den hata");
            }
            }
    }
    
    
}
