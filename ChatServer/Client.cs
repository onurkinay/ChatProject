using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class Client
    {
        public int id = -1;
        public string nickname = "";
        public TcpClient user_tcpclient;

        public Client(TcpClient tcpclient)
        {
            nickname = "***User coming***";
            user_tcpclient = tcpclient;
        }
       
       override
       public string ToString()
        {
            return nickname;
        }
    }
}
