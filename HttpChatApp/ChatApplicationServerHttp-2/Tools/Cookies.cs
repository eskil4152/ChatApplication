using System;
using System.Net;
using System.Text;
namespace ChatApplicationServerHttp
{
	public class Cookies
	{
		private static readonly string key = "secret";

		public static Cookie CreateCookie(string name, string value)
		{
            Cookie cookie = new(name, Security.Encrypt(value, key))
            {
                Path = "/",
                Expires = DateTime.Now.AddDays(1),
                HttpOnly = true,
				Secure = true,
            };

            return cookie;
		}

		public static string DecryptCookie(Cookie cookie)
		{
			return Security.Decrypt(cookie.Value, key);
		}
	}
}

