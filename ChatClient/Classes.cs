﻿ 
using System;
using System.Windows;
using System.Windows.Controls; 

namespace ChatClient
{

    class Classes
    {
    }

    public class sOda
    {
        public int id;
        public string name;
        public sOda(int kid, string kname)
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
        public Message(Uye uye, string mesaj)
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


}
