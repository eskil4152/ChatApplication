using System;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

public struct HttpMessage
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required MessageType Type { get; set; }
    public required string Username { get; set; }
    public string? Password { get; set; }
    public string? key { get; set; }
    public required string Message { get; set; }
    public int? RoomNumber { get; set; }
}

public enum MessageType
{
    [EnumMember(Value = "LOGIN")]
    LOGIN,
    [EnumMember(Value = "ROOMSELECT")]
    ROOMSELECT,
    [EnumMember(Value = "KEY")]
    KEY,
    [EnumMember(Value = "CHAT")]
    CHAT
}
