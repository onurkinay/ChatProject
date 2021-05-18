 
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ChatClient
{

    class Classes
    {
    }

    public class dosyaBilgileri
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

    public class classOda
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
    public class Uye
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

    public class Message
    {
        
        public Uye uye { get; set; }
        public string mesaj { get; set; }
        public bool dosyaMi { get; set; }
        public bool gonderilmisMi { get; set; }
        public Oda oda { get; set; }
        
        public Message(Uye uye, string mesaj, Oda oda=null)
        {
            this.dosyaMi = false;
            if (mesaj.Contains("###dosyaVar###"))
            {
                this.dosyaMi = true;
                gonderilmisMi = false;
                mesaj = mesaj.Replace("###dosyaVar###dosyaAdi=", "");

            }else if (mesaj.Contains("###gonderilmisDosya###"))
            {
                this.dosyaMi = true;
                this.gonderilmisMi = true;
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

    public static class MyClass
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

    public static class FlowDocumentPagePadding
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
