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

        override
      public String ToString()
        {
            return nickname;
        }
    }
}
