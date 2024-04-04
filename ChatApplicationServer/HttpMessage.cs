using System;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public class ChatMessage
{
    public required string Username { get; set; }
    public required string Message { get; set; }
    public required string RoomName { get; set; }
}

public class LoginMessage
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RoomMessage
{
    public required string RoomName { get; set; }
    public string? RoomPassword { get; set; }
}