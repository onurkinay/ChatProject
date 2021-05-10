using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Classes
    {
    }

    public class Oda
    {
        public int id;
        public string name;
        public Oda(int kid, string kname)
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
