using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
	public class UserActions
	{
        public static List<Room>? CreateUser(DatabaseService databaseService, LoginMessage loginMessage)
        {
            if (loginMessage.LoginType == LoginType.LOGIN)
            {
                User? user = databaseService.CheckUser(loginMessage.Username, loginMessage.Password);

                if (user != null)
                {
                    return user.Rooms;
                }
                else
                {
                    Console.WriteLine("Invalid login");
                    return null;
                }
            }
            else if (loginMessage.LoginType == LoginType.REGISTER)
            {
                User user = new()
                {
                    Username = loginMessage.Username,
                    Password = Password.HashPassword(loginMessage.Password),
                    Rooms = new List<Room>(),
                };

                if (databaseService.Register(user))
                {
                    return new List<Room>();
                }

                Console.WriteLine("Registration failed");
                return null;
            }
            else
            {
                Console.WriteLine("Invalid login type: " + loginMessage.LoginType);
                return null;
            }
        }
    }
}

