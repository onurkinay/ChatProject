using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class Client
    {
        public int id = (new Random()).Next(1000,9999);
        public string nickname = "";
        public TcpClient user_tcpclient;

        public Client(TcpClient tcpclient)
        {
            nickname = "User#" + id.ToString();
            user_tcpclient = tcpclient;
        }
       
       override
       public string ToString()
        {
            return nickname;
        }
    }
}
