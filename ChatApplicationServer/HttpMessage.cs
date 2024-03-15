using System;

namespace ChatApplicationServer
{
    public class HttpMessage
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Message { get; set; }
        public int? RoomNumber { get; set; }
    }
}