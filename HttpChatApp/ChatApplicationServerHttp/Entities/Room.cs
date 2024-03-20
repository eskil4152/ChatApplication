using System.Net.WebSockets;
using System.Text;

namespace ChatApplicationServerHttp
{
    public class Room
    {
        public string? RoomName { get; set; }
        public int? RoomNumber { get; set; }
        public string? RoomPassword { get; set; }
        public string? RoomSecret { get; set; }
        // Use websocket only for active members
        public List<WebSocket>? MembersActive { get; set; }
        public List<User>? Members { get; set; }
        public List<string>? Messages { get; set; }
    }
}