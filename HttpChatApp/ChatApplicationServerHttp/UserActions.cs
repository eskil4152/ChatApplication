using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
	public class UserActions
	{
        public static User? Login(DatabaseService databaseService, LoginMessage loginMessage)
        {
            return databaseService.Login(loginMessage.Username, loginMessage.Password);
        }

        public static User? Register(DatabaseService databaseService, LoginMessage loginMessage)
        {
            User user = new()
            {
                Username = loginMessage.Username,
                Password = Security.HashPassword(loginMessage.Password),
                Rooms = new List<Room>(),
            };

            return databaseService.Register(user) ? user : null;
        }
    }
}

