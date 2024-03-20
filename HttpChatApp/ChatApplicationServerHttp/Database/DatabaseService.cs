﻿using System;
namespace ChatApplicationServerHttp
{
	public class DatabaseService
	{
		private readonly DatabaseContext databaseContext;

		public DatabaseService(DatabaseContext databaseContext)
		{
            this.databaseContext = databaseContext;
		}

        public bool Register(User user)
		{
			if (databaseContext.users.FirstOrDefault(u => u.Username == user.Username) != null)
			{
				return false;
			}

			databaseContext.users.Add(user);
			databaseContext.SaveChanges();
			return true;
		}

		public User? CheckUser(string username, string password)
		{
			// find by username
			// check found user with provided user password
			// return if correct
			User? user = databaseContext.users.FirstOrDefault(u => u.Username == username);

			if (user != null && Password.CheckPassword(password, user.Password))
			{
				return user;
			}

			return null;
        }

		public void GetMessages()
		{
			// get messages from a room
		}

		public void SaveMessages()
		{
			// save new messages to room
			// perhaps save messages in server and only save once an hour
		}

		public void UserAndRoomConnect()
		{
			// when user add room, add room to user list and user to room list
		}
	}
}

