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
			return new Cookie(name, Security.Encrypt(value, key));
		}

		public static string DecryptCookie(Cookie cookie)
		{
			return Security.Decrypt(cookie.Value, key);
		}
	}
}

