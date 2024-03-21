﻿using System;
using System.Net.WebSockets;

namespace ChatApplicationServerHttp
{
	public class UserActions
	{
        public static List<Room>? LoginRegister(DatabaseService databaseService, LoginMessage loginMessage)
        {
            if (loginMessage.LoginType == LoginType.LOGIN)
            {
                User? user = databaseService.Login(loginMessage.Username, loginMessage.Password);

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
                    Console.WriteLine("Got it");
                    User? user1 = databaseService.Login(loginMessage.Username, loginMessage.Password);
                    if (user1 == null)
                    {
                        Console.WriteLine("Was null");
                    } else
                    {
                        Console.WriteLine("Was not null");
                    }
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
