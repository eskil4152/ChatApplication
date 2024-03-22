using System;
using System.ComponentModel.DataAnnotations;
namespace ChatApplicationServerHttp
{
	public class RoomUser
	{
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
    }
}

