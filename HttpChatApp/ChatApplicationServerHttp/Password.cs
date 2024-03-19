using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplicationServerHttp
{
	public static class Password
	{
		public static string HashPassword(string password)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(password);
			byte[] hashedBytes;

			using (SHA256 sha256 = SHA256.Create())
			{
				hashedBytes = sha256.ComputeHash(bytes);
			}

			StringBuilder hashBuilder = new();

			foreach (byte b in hashedBytes)
			{
				hashBuilder.Append(b.ToString("x2"));
			}

			return hashBuilder.ToString();
		}

		public static bool CheckPassword(string enteredPassword, string hashedPassword)
		{
            byte[] bytes = Encoding.UTF8.GetBytes(enteredPassword);
            byte[] hashedBytes;

            using (SHA256 sha256 = SHA256.Create())
            {
                hashedBytes = sha256.ComputeHash(bytes);
            }

            StringBuilder hashBuilder = new();

            foreach (byte b in hashedBytes)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

			return hashBuilder.ToString() == hashedPassword;
        }
	}
}

