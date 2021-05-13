 
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
        public Message(Uye uye, string mesaj)
        {
            this.dosyaMi = false;
            if (mesaj.Contains("###dosyaVar###"))
            {
                this.dosyaMi = true;
                mesaj = mesaj.Replace("###dosyaVar###dosyaAdi=", "");
               
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

    
}
