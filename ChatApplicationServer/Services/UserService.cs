using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
	public class UserService
	{
        private readonly DatabaseService databaseService;

        public UserService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public User? Login(LoginMessage loginMessage)
        {
            return databaseService.Login(loginMessage.Username, loginMessage.Password);
        }

        public User? Register(LoginMessage loginMessage)
        {
            User user = new()
            {
                Username = loginMessage.Username,
                Password = Security.HashPassword(loginMessage.Password),
            };

            return databaseService.Register(user) ? user : null;
        }

        public User? GetUser(string encryptedUsername)
        {
            return databaseService.GetUser(Security.Decrypt(encryptedUsername, "key"));
        }
    }
}

