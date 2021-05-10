using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{

    /// <summary>
    /// odalar, bir txt dosyasında tutulabilir.
    /// genel, normal oda, özel olarak kullanılacaktır
    /// </summary>
    public class Oda
    {
        public int id = (new Random()).Next(1000, 9999);
        public List<Client> bulunanlar = new List<Client>();
        public string name;
        public int tur = 0;//0 -> genel, 1->oda, 2-> özel

        public Oda(string isim)
        {
            name = isim;
        }

    }
}
