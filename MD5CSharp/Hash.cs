using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MD5Hash
{
    public static class Hash
    {
        public static string Content(string text)
        {
            return HashBuilder(text);
        }

        public static string Content(this byte[] byteArray)
        {
            return HashBuilder(byteArray);
        }

        public static string Content(this Stream stream)
        {
            return HashBuilder(stream);
        }

        public static string GetMD5(this string text)
        {
            return HashBuilder(text);
        }

        public static string GetMD5(this byte[] byteArray)
        {
            return HashBuilder(byteArray);
        }

        public static string GetMD5(this Stream stream)
        {
            return HashBuilder(stream);
        }

        private static string HashBuilder(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        private static string HashBuilder(byte[] byteArray)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(byteArray);
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        private static string HashBuilder(Stream stream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(stream);
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}
