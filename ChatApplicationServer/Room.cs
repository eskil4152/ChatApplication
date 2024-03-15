using System;
namespace ChatApplicationServer
{
	public struct Room
	{
		string RoomName { get; set; }
        int RoomNumber { get; set; }
        string RoomPassword { get; set; }
        string RoomSecret { get; set; }
        Member[] members { get; set; }
    }

	public struct Member
	{
		string Username { get; set; }
	}
}

