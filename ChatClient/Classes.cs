using System;

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
        public string id;
        public string nickname;

        public Uye(string kid, string kname)
        {
            id = kid;
            nickname = kname;
        }
        override
      public String ToString()
        {
            return nickname;
        }
    }

    public class Message
    {
        public Uye uye;
        public string mesaj;
        public Message(Uye uye, string mesaj)
        {
            this.uye = uye;
            this.mesaj = mesaj;
        }
        override
        public string ToString()
        {
            return uye.nickname+": "+mesaj;
        }

        public string getTcpFormat()
        {
            return uye.id+": "+mesaj;
        }
    }
}
