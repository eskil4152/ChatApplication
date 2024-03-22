using System;
using System.Net.WebSockets;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationServerHttp
{
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }

        public required List<Room> Rooms { get; set; }
    }
}

