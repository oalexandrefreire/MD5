using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MD5Hash
{
    public static class Hash
    {
        public static string Content(string text, EncodingType encodingType = EncodingType.ASCII) => GetHash(text, encodingType);

        public static string GetMD5(this string text, EncodingType encodingType = EncodingType.ASCII) => GetHash(text, encodingType);

        public static string GetMD5(this object value, EncodingType encodingType = EncodingType.UTF8)
        {
            try
            {
                string text = JsonConvert.SerializeObject(value);
                return GetHash(text, encodingType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMD5: {ex.Message}");
                return null;
            }
        }

        public static string GetMD5(this byte[] byteArray) => HashBuilder(byteArray);

        public static string GetMD5(this Stream stream) => HashBuilder(stream);

        private static string GetHash(this string text, EncodingType encodingType = EncodingType.ASCII)
        {
            byte[] bytes = GetBytesWithEncoding(text, encodingType);

            if (bytes == null)
                return null;

            return HashBuilder(bytes);
        }

        private static byte[] GetBytesWithEncoding(string text, EncodingType encodingType)
        {
            Encoding encoding = GetEncoding(encodingType);

            if (encoding == null)
                return null;

            return encoding.GetBytes(text);
        }

        private static Encoding GetEncoding(EncodingType encodingType)
        {
            switch (encodingType)
            {
                case EncodingType.UTF8:
                    return Encoding.UTF8;
                case EncodingType.UTF7:
                    return Encoding.UTF7;
                case EncodingType.UTF32:
                    return Encoding.UTF32;
                case EncodingType.Unicode:
                    return Encoding.Unicode;
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode;
                case EncodingType.ASCII:
                    return Encoding.ASCII;
                case EncodingType.Default:
                    return Encoding.Default;
                default:
                    return null;
            }
        }

        private static string HashBuilder(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(data);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }

        private static string HashBuilder(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] result = md5.ComputeHash(stream);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }
    }
}