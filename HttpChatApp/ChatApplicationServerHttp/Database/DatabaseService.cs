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

			List<Guid> roomIds = databaseContext.roomuser
				.Where(ru => ru.UserId == user.Id)
				.Select(ru => ru.RoomId)
				.ToList();

			List<Room> rooms = databaseContext.rooms
				.Where(r => roomIds.Contains(r.Id))
				.ToList();

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
				Messages = new List<string>() { "Welcome to your new room!" },
				Members = new List<Guid>() { user.Id },
			};

            databaseContext.rooms.Add(room);

            RoomUser roomUser = new()
            {
                UserId = user.Id,
                RoomId = room.Id
            };
            databaseContext.roomuser.Add(roomUser);

            Console.WriteLine("create: " + JsonSerializer.Serialize(room));

			databaseContext.rooms.Add(room);
            databaseContext.SaveChanges();
			
			return true;
		}

		public bool JoinRoom(RoomMessage roomMessage, User user)
		{
			Room? room = databaseContext.rooms.FirstOrDefault(u => u.RoomName == roomMessage.RoomName);

			if (room != null && room.RoomPassword == roomMessage.RoomPassword)
			{
				room.Members.Add(user.Id);
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

		public Room? GetRoomByName(string roomName)
		{
			return databaseContext.rooms.FirstOrDefault(r => r.RoomName == roomName);
		}

		public void AddMessageToRoom(Room room, string message)
		{
			Room? dbRoom = databaseContext.rooms.Find(room);
			if (dbRoom == null) return;

			dbRoom.Messages.Add(message);
			databaseContext.SaveChanges();

			// perhaps save messages in server and only save once an hour
		}
	}
}

