using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace ChatClient
{

    public class Classes
    {
        public static List<string> EmojiList()
        {//EMOJİ LİSTESİ


            List<string> _List = new List<string>();
            //üçlü
            _List.Add(">:(,😡");
            _List.Add(":-),🙂");
            _List.Add(":-],🙂");
            _List.Add(":-D,😃");
            _List.Add("X-D,😃");
            _List.Add("x-D,😃");
            _List.Add("8-D,😃");
            _List.Add(":-(,☹️");
            _List.Add(":'(,😭");
            _List.Add(":'-(,😭");
            _List.Add(":'),😂");
            _List.Add(":-o,😮");
            _List.Add(":-O,😮");
            _List.Add(":->,😊");
            _List.Add(":-3,🙂");
            _List.Add("8-),🙂");
            _List.Add(":-},🙂");
            _List.Add(":-*,😘");
            _List.Add(";‑),😉");
            _List.Add(":-J,😏");
            _List.Add("%‑),😵");
            _List.Add("B-),😎");
            _List.Add(">:),😈");
            _List.Add("}:),😈");
            _List.Add("3:),😈");
            _List.Add(">:3,😈");
            _List.Add("O:),😇");
            _List.Add(":-X,🤐");
            _List.Add(":-#,🤐");
            _List.Add(":-#,😕");
            _List.Add("O:),😇");
            _List.Add("O:3,😇");
            _List.Add("0:),😇");
            _List.Add("://,😞");
            _List.Add(":-P,😛");
            _List.Add("X-P,😝");
            _List.Add("X-p,😝");
            _List.Add("D:<,😨");
            _List.Add(";-),😉");
            //ikili
            _List.Add(":),🙂");
            _List.Add(":},🙂");
            _List.Add("8),🙂");
            _List.Add(":],🙂");
            _List.Add(":3,🙂");
            _List.Add(":D,😃");
            _List.Add("=D,😃");
            _List.Add(":3,😃");
            _List.Add("8D,😃");
            _List.Add("xD,😃");
            _List.Add("XD,😃");
            _List.Add(":(,☹️");
            _List.Add(":O,😮");
            _List.Add(":o,😮");
            _List.Add(":>,😊");
            _List.Add(":*,😘");
            _List.Add(":x,😘");
            _List.Add(";),😉");
            _List.Add("%),😵");
            _List.Add(":X,🤐");
            _List.Add(":#,🤐");
            _List.Add(":/,😕");
            _List.Add(":E,😬");
            _List.Add(";3,😈");
            _List.Add(":&,😶");
            _List.Add(":$,😶");
            _List.Add(":|,😐");
            _List.Add(":P,😛");
            _List.Add(":p,😝");
            _List.Add("D:,😨");
            _List.Add("D8,😨");
            _List.Add("D=,😨");
            return _List;
        }
    }

    

    public class dosyaBilgileri //sunucudan gelen dosya bilgileri için
    {
        public string fileName;
        public string safeFileName;
        public object alici;

        public dosyaBilgileri(string safeFileName, string fileName, object alici)
        {
            this.fileName = fileName;
            this.safeFileName = safeFileName;
            this.alici = alici;
        }
    }

    public class classOda//oda window sınıfı direk listeye eklenmediği için oluşturuldu
    {
        public int id;
        public string name;
        public classOda(int kid, string kname)
        {
            id = kid;
            name = kname;
        }
        override
        public String ToString()
        {
            return name;
        }
    }
    public class Uye // uye classı
    {
        public string id { get; set; }
        public string nickname { get; set; }
        public bool Selected { get; set; }
        public bool DoBlink { get; set; }
        public Uye(string kid, string kname)
        {
            id = kid;
            nickname = kname;
            Selected = true;
            DoBlink = false;
        }
        override
      public String ToString()
        {
            return nickname;
        }
    }

    public class Message //mesaj sınıfı
    {
        
        public Uye uye { get; set; }
        public string mesaj { get; set; }
        public bool dosyaMi { get; set; }
        public bool gonderilmisMi { get; set; }
        public int dosyaId { get; set; }
        public Oda oda { get; set; }
        
        public Message(Uye uye, string mesaj, Oda oda=null)
        {
            this.dosyaMi = false;
            if (mesaj.Contains("###dosyaVar###"))//gelen mesaj bir dosya mı
            {
                this.dosyaMi = true;
                gonderilmisMi = false;
                dosyaId = Convert.ToInt32(mesaj.Split('*')[1] ) ;
                mesaj = mesaj.Replace("*"+dosyaId,"");
                mesaj = mesaj.Replace("###dosyaVar###dosyaAdi=", "");

            }else if (mesaj.Contains("###gonderilmisDosya###"))//önceden gönderilmiş bir dosya mı
            {
                this.dosyaMi = true;
                this.gonderilmisMi = true;
                dosyaId = Convert.ToInt32(mesaj.Split('*')[1]);
                mesaj = mesaj.Replace("*" + dosyaId, "");
                mesaj = mesaj.Replace("###gonderilmisDosya###dosyaAdi=", "");
            }
           
            this.uye = uye;
            this.mesaj = mesaj;
            this.oda = oda;
        }
        override
        public string ToString()
        {
            return uye.nickname + ": " + mesaj;
        }

        public string getTcpFormat()
        {
            return uye.id + ": " + mesaj;
        }
         
    }

    public static class MyClass //window control'e ek obje değişken alanı açar
    {

        public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.RegisterAttached("MyProperty",
            typeof(object), typeof(MyClass), new FrameworkPropertyMetadata(null));


        public static object GetMyProperty(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (object)element.GetValue(MyPropertyProperty);
        }
        public static void SetMyProperty(UIElement element, object value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(MyPropertyProperty, value);
        }
    }

    public static class FlowDocumentPagePadding //mesaj kutuların görünümü sıfırlar
    {
        public static Thickness GetPagePadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(PagePaddingProperty);
        }
        public static void SetPagePadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(PagePaddingProperty, value);
        }
        public static readonly DependencyProperty PagePaddingProperty =
            DependencyProperty.RegisterAttached("PagePadding", typeof(Thickness), typeof(FlowDocumentPagePadding), new UIPropertyMetadata(new Thickness(double.NegativeInfinity), (o, args) =>
            {
                var fd = o as FlowDocument;
                if (fd == null) return;
                var dpd = DependencyPropertyDescriptor.FromProperty(FlowDocument.PagePaddingProperty, typeof(FlowDocument));
                dpd.RemoveValueChanged(fd, PaddingChanged);
                fd.PagePadding = (Thickness)args.NewValue;
                dpd.AddValueChanged(fd, PaddingChanged);
            }));
        public static void PaddingChanged(object s, EventArgs e)
        {
            ((FlowDocument)s).PagePadding = GetPagePadding((DependencyObject)s);
        }
    }

}
