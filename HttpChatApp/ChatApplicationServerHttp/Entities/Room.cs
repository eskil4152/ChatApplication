using System.Net.WebSockets;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationServerHttp
{
    public class Room
    {
        public Room()
        {
            Id = Guid.NewGuid();
            Members = new List<User>();
            Messages = new List<string>();
        }

        [Key]
        public Guid Id { get; set; }
        public string? RoomName { get; set; }
        public string? RoomPassword { get; set; }

        // Use websocket only for active members
        //public List<WebSocket>? MembersActive { get; set; }
        public required List<User> Members { get; set; }
        public required List<string> Messages { get; set; }
    }
}