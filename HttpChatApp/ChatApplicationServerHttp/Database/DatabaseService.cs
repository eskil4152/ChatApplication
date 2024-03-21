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

		public List<Room> GetRooms(string username)
		{
			return databaseContext.users.FirstOrDefault(u => u.Username == username).Rooms;
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

		public bool JoinRoom(RoomMessage roomMessage, User user)
		{
			Room? room = databaseContext.rooms.FirstOrDefault(u => u.RoomName == roomMessage.RoomName);

			if (room != null && room.RoomPassword == roomMessage.RoomPassword)
			{
				room.Members.Add(user);
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
			User? user = databaseContext.users.FirstOrDefault(u => u.Username == username);

			if (user != null && Security.CheckPassword(password, user.Password))
			{
				return user;
			}

			return null;
        }

		public User? GetUser(string username)
		{
			return databaseContext.users.FirstOrDefault(u => u.Username == username);
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
	}
}

