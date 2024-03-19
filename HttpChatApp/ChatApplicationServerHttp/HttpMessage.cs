using System;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public abstract class HttpMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required MessageType Type { get; set; }
    /*
    public required string Username { get; set; }
    public string? Password { get; set; }
    public string? key { get; set; }
    public required string Message { get; set; }
    public int? RoomNumber { get; set; }
    */
}

public class ChatMessage : HttpMessage
{
    public required string Username { get; set; }
    public required string Message { get; set; }
    public int? RoomNumber { get; set; }
}

public class LoginMessage : HttpMessage
{
    public required LoginType LoginType { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class Other : HttpMessage
{
    public required string Username { get; set; }
    public string? Password { get; set; }
    public string? key { get; set; }
    public required string Message { get; set; }
    public int? RoomNumber { get; set; }
}

public enum LoginType
{
    [EnumMember(Value = "LOGIN")]
    LOGIN,
    [EnumMember(Value = "REGISTER")]
    REGISTER
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
