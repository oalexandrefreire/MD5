using MD5Hash.Test.Models;
using MD5Hash;
using System.IO;
using System.Text;
using Xunit;

namespace MD5.Test
{
    public class HashTest
    {
        [Fact]
        public void MD5HashContent()
        {
            var hash = Hash.Content("123");
            Assert.Equal("202cb962ac59075b964b07152d234b70", hash);
        }

        [Fact]
        public void MD5HashGetMD5()
        {
            string hash1 = "123";
            string hash2 = "Hello! 👋 こんにちは！你好！안녕하세요! Здравствуйте! مرحبًا! Hola! Bonjour! Hallo! नमस्ते! Γεια σας! שלום! Cześć! Привет! Merhaba! สวัสดีครับ! مرحبًا بك! 你好嗎？🌍\r\n\r\nThis is a test string with a mix of characters and emojis. 🌟🎉 Let's include some special characters as well: @#$%^&*()_+[]{}|;:'\",.<>?/`~-=_\\😊\r\n\r\n🐱‍🏍 Feel free to use this string for testing purposes. If you have any specific characters, emojis, or languages you'd like to include, just let me know! 🚀";
            Assert.Equal("202cb962ac59075b964b07152d234b70", hash1.GetMD5());
            Assert.Equal("e2f6b5c9fef1da83cbdeab8b36d3ddbd", hash2.GetMD5(EncodingType.UTF8));
        }

        [Fact]
        public void MD5HashGetMD5FromByteArray()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Hello, World!");
            string expectedHash = "65a8e27d8879283831b664bd8b7f0ad4";
            string actualHash = byteArray.GetMD5();
            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void MD5HashGetMD5FromStream()
        {
            var stream = File.OpenRead("Rondonia.pdf");
            string expectedHash = "580a9bb265b985a41df74ad34f4d8951";
            string actualHash = stream.GetMD5();
            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void MD5HashGetMD5FromObject()
        {
            var obj = new BrasilModel() { Id = 1, Details = "Maior país da América do Sul" };
            string expectedHash = "a7f13adac4087d45065c853c02baa37e";
            string actualHash = obj.GetMD5();
            Assert.Equal(expectedHash, actualHash);
        }
    }
}