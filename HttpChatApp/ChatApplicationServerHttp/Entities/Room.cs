using System.Net.WebSockets;
using System.ComponentModel.DataAnnotations;

namespace ChatApplicationServerHttp
{
    public class Room
    {
        public Room()
        {
            Id = Guid.NewGuid();
            Members = new List<Guid>();
        }

        [Key]
        public Guid Id { get; set; }
        public required string RoomName { get; set; }
        public required string RoomPassword { get; set; }
        public required List<string> Messages { get; set; }

        public required List<Guid> Members { get; set; }
    }
}