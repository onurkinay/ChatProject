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
        public int id;
        public string nickname;

        public Uye(int kid, string kname)
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
}
