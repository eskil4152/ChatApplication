using System;
using System.Security.Cryptography;
using System.Text;

static class Crypto
{
    public static string Encrypt(string s, string key)
    {
        if (String.IsNullOrEmpty(s) || String.IsNullOrEmpty(key))
        {
            return s;
        }

        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new((Stream)cryptoStream))
            {
                streamWriter.Write(s);
            }

            array = memoryStream.ToArray();
        }

        return Convert.ToBase64String(array);
    }


    public static string Decrypt(string key, string s)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(s);

        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        try
        {
            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new((Stream)cryptoStream);
            return streamReader.ReadToEnd();
        }
        catch
        {
            return "Banana?";
        }
    }
}