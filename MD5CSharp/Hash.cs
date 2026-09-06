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

        public static string GetMD5(this string text, EncodingType encodingType = EncodingType.ASCII) => GetHash(text, encodingType);

        public static string GetSHA256(this object value, EncodingType encodingType = EncodingType.UTF8)
        {
            try
            {
                string text = JsonConvert.SerializeObject(value);
                return GetSHA256Hash(text, encodingType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSHA256: {ex.Message}");
                return null;
            }
        }

        public static string GetSHA256(this byte[] byteArray) => SHA256HashBuilder(byteArray);

        public static string GetSHA256(this Stream stream) => SHA256HashBuilder(stream);

        public static string GetSHA256(this string text, EncodingType encodingType = EncodingType.UTF8) => GetSHA256Hash(text, encodingType);

        public static string GetHMACSHA256(this object value, string key, EncodingType encodingType = EncodingType.UTF8)
        {
            try
            {
                if (key == null)
                    return null;

                string text = JsonConvert.SerializeObject(value);
                return GetHMACSHA256Hash(text, key, encodingType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetHMACSHA256: {ex.Message}");
                return null;
            }
        }

        public static string GetHMACSHA256(this byte[] byteArray, byte[] key) => HMACSHA256HashBuilder(byteArray, key);

        public static string GetHMACSHA256(this Stream stream, byte[] key) => HMACSHA256HashBuilder(stream, key);

        public static string GetHMACSHA256(this string text, string key, EncodingType encodingType = EncodingType.UTF8) => GetHMACSHA256Hash(text, key, encodingType);

        public static string GetMD5WithSalt(this string text, string salt, EncodingType encodingType = EncodingType.UTF8)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            string saltedText = $"{salt}{text}";
            return GetHash(saltedText, encodingType);
        }

        public static string GetMD5WithSalt(this object value, string salt, EncodingType encodingType = EncodingType.UTF8)
        {
            try
            {
                string text = JsonConvert.SerializeObject(value);
                string saltedText = $"{salt}{text}";
                return GetHash(saltedText, encodingType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMD5WithSalt: {ex.Message}");
                return null;
            }
        }

        public static string GetMD5WithSalt(this byte[] byteArray, byte[] salt)
        {
            if (byteArray == null || salt == null)
                return null;

            byte[] saltedBytes = Combine(byteArray, salt);
            return HashBuilder(saltedBytes);
        }

        public static string GetMD5WithSalt(this Stream stream, byte[] salt)
        {
            return HashBuilder(stream, salt);
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }

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
                return ToLowerHex(md5.ComputeHash(data));
            }
        }

        private static string HashBuilder(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                return ToLowerHex(md5.ComputeHash(stream));
            }
        }

        private static string HashBuilder(Stream stream, byte[] suffix)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] buffer = new byte[81920];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    }

                    md5.TransformFinalBlock(suffix, 0, suffix.Length);
                    return ToLowerHex(md5.Hash);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMD5WithSalt: {ex.Message}");
                return null;
            }
        }

        private static string GetSHA256Hash(string text, EncodingType encodingType)
        {
            if (text == null)
                return null;

            byte[] bytes = GetBytesWithEncoding(text, encodingType);
            return bytes == null ? null : SHA256HashBuilder(bytes);
        }

        private static string GetHMACSHA256Hash(string text, string key, EncodingType encodingType)
        {
            if (text == null || key == null)
                return null;

            byte[] data = GetBytesWithEncoding(text, encodingType);
            byte[] keyBytes = GetBytesWithEncoding(key, encodingType);

            if (data == null || keyBytes == null)
                return null;

            return HMACSHA256HashBuilder(data, keyBytes);
        }

        private static string SHA256HashBuilder(byte[] data)
        {
            if (data == null)
                return null;

            using (SHA256 sha256 = SHA256.Create())
            {
                return ToLowerHex(sha256.ComputeHash(data));
            }
        }

        private static string SHA256HashBuilder(Stream stream)
        {
            if (stream == null)
                return null;

            using (SHA256 sha256 = SHA256.Create())
            {
                return ToLowerHex(sha256.ComputeHash(stream));
            }
        }

        private static string HMACSHA256HashBuilder(byte[] data, byte[] key)
        {
            if (data == null || key == null)
                return null;

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return ToLowerHex(hmac.ComputeHash(data));
            }
        }

        private static string HMACSHA256HashBuilder(Stream stream, byte[] key)
        {
            if (stream == null || key == null)
                return null;

            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return ToLowerHex(hmac.ComputeHash(stream));
            }
        }

        private static string ToLowerHex(byte[] data)
        {
            const string hexadecimal = "0123456789abcdef";
            char[] result = new char[data.Length * 2];

            for (int i = 0; i < data.Length; i++)
            {
                result[i * 2] = hexadecimal[data[i] >> 4];
                result[(i * 2) + 1] = hexadecimal[data[i] & 0x0F];
            }

            return new string(result);
        }
    }
}
