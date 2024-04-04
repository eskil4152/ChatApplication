using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplicationServerHttp
{
	public static class Security
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

        public static string Encrypt(string input, string key)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            byte[] encryptedBytes = new byte[inputBytes.Length];
            for (int i = 0; i < inputBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string input, string key)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            byte[] decryptedBytes = new byte[inputBytes.Length];
            for (int i = 0; i < inputBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

