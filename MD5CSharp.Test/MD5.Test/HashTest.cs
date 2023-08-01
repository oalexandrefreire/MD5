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
            string hash = "123";
            Assert.Equal("202cb962ac59075b964b07152d234b70", hash.GetMD5());
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
            var stream = File.OpenRead("Rondônia.pdf");
            string expectedHash = "580a9bb265b985a41df74ad34f4d8951";
            string actualHash = stream.GetMD5();
            Assert.Equal(expectedHash, actualHash);
        }
    }
}