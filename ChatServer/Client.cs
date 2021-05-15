using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChatServer
{
    public class Client
    {
        public int id = -1;
        public string nickname = "";
        public TcpClient user_tcpclient;

        public List<string> gelenDosyaParcaciklari = new List<string>();

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
