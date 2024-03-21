using System;
namespace ChatApplicationServerHttp
{
	public class DatabaseService
	{
		private readonly DatabaseContext databaseContext;

		public DatabaseService(DatabaseContext databaseContext)
		{
            this.databaseContext = databaseContext;
		}

		public bool CreateRoom(Room room)
		{
            if (databaseContext.rooms.FirstOrDefault(u => u.RoomName == room.RoomName) != null)
			{
				return false;
			}

			databaseContext.rooms.Add(room);
			databaseContext.SaveChanges();
			return true;
		}

		public bool JoinRoom(RoomMessage roomMessage, UserData userData)
		{
			Room? room = databaseContext.rooms.FirstOrDefault(u => u.RoomName == roomMessage.RoomName);

			if (room != null && room.RoomPassword == roomMessage.RoomPassword && userData.user != null)
			{
				room.Members.Add(userData.user);
				databaseContext.SaveChanges();

				return true;
			}

			return false;
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

		public User? Login(string username, string password)
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

