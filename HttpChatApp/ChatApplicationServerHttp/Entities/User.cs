using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        //public WebSocket WebSocket { get; set; }
        public List<Room> Rooms { get; set; }
    }
}

