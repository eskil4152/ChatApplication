using System;

public class HttpMessage
{
    public required string Username { get; set; }
    public string? Password { get; set; }
    public string? Message { get; set; }
    public required int RoomNumber { get; set; }
}
