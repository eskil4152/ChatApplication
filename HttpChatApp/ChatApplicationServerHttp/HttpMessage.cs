using System;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public class ChatMessage
{
    public required string Username { get; set; }
    public required string Message { get; set; }
    public int? RoomNumber { get; set; }
}

public class LoginMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required LoginType LoginType { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RoomMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required RoomType RoomType { get; set; }
    public string? RoomName { get; set; }
    public string? RoomPassword { get; set; }
    public required string Username { get; set; }
}

public enum LoginType
{
    [EnumMember(Value = "LOGIN")]
    LOGIN,
    [EnumMember(Value = "REGISTER")]
    REGISTER
}

public enum RoomType
{
    [EnumMember(Value = "JOIN")]
    JOIN,
    [EnumMember(Value = "CREATE")]
    CREATE
}

public enum MessageType
{
    [EnumMember(Value = "LOGIN")]
    LOGIN,
    [EnumMember(Value = "JOINROOM")]
    JOINROOM,
    [EnumMember(Value = "KEY")]
    KEY,
    [EnumMember(Value = "CHAT")]
    CHAT
}
