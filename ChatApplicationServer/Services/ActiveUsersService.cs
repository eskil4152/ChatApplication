using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
	public class ActiveUsersService
	{
        public readonly ConcurrentDictionary<Guid, List<WebSocket>> activeUsers = new();

        public void AddActiveUser(Guid guid, WebSocket webSocket)
        {
            if (activeUsers.ContainsKey(guid))
            {
                activeUsers[guid].Add(webSocket);
            } else
            {
                activeUsers.TryAdd(guid, new List<WebSocket>() { webSocket });
            }
        }

        public void RemoveActiveUser(Guid guid, WebSocket webSocket)
        {
            activeUsers[guid].Remove(webSocket);
        }

        public List<WebSocket> GetActiveUsers(Guid guid)
        {
            return activeUsers[guid];
        }
    }
}

