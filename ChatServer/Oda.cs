using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{

    /// <summary>
    /// odalar, bir txt dosyasında tutulabilir.
    ///  
    /// </summary>
    public class Oda
    {
        public int id = (new Random()).Next(1000, 9999);
        public List<Client> bulunanlar = new List<Client>();
        public string name;
        public Client olusturan;
        string fileName = "";

        public Oda(string isim, Client cOlusturan)
        {
            name = isim;
            olusturan = cOlusturan;
            fileName = @"odalar/oda-"+id+".txt";
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                // Create a new file     
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file    
                    Byte[] title = new UTF8Encoding(true).GetBytes("New Text File");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("Mahesh Chand");
                    fs.Write(author, 0, author.Length);
                }

                // Open the stream and read it back.    
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        override
        public string ToString()
        {
            return this.name;
        }

    }
}
