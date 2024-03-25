using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
    public class UserData
    {
        public required WebSocket WebSocket;
        public User? user = null;
    }
}

