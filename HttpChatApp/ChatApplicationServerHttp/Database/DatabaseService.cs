using System;
using System.Text.Json;
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
			User? user = databaseContext.users.FirstOrDefault(u => u.Username == username);

			if (user == null) return new List<Room>();

            List<RoomUser> roomUser = databaseContext.roomuser.Where(u => u.UserId == user.Id).ToList();

			List<Room> rooms = new();
			foreach (RoomUser ru in roomUser)
			{
				rooms.Add(databaseContext.rooms.FirstOrDefault(r => r.Id == ru.RoomId));
			}

            return rooms;
		}

		public bool CreateRoom(RoomMessage roomMessage, User user)
		{
            
            if (databaseContext.rooms.FirstOrDefault(u => u.RoomName == roomMessage.RoomName) != null)
			{
				return false;
			}

            
            Room room = new()
			{
				RoomName = roomMessage.RoomName,
				RoomPassword = roomMessage.RoomPassword,
				Members = new List<User>() { user },
				Messages = new List<string>() { "Welcome to your new room!" },
			};

            
            databaseContext.rooms.Add(room);


			RoomUser roomUser = new()
			{
				RoomId = room.Id,
				UserId = user.Id,
			};

            
            databaseContext.roomuser.Add(roomUser);

            databaseContext.SaveChanges();
			
			return true;
		}

		public bool JoinRoom(RoomMessage roomMessage, User user)
		{
			Room? room = databaseContext.rooms.FirstOrDefault(u => u.RoomName == roomMessage.RoomName);

			if (room != null && room.RoomPassword == roomMessage.RoomPassword)
			{
				room.Members.Add(user);
				user.Rooms.Add(room);

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

