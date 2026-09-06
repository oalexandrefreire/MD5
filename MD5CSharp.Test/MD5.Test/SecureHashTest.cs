using MD5Hash;
using System.IO;
using System.Text;
using Xunit;

namespace MD5.Test
{
    public class SecureHashTest
    {
        [Fact]
        public void GetSHA256_String_ShouldMatchKnownVector()
        {
            string hash = "hello world".GetSHA256();

            Assert.Equal("b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9", hash);
        }

        [Fact]
        public void GetSHA256_ByteArray_ShouldMatchStringResult()
        {
            byte[] data = Encoding.UTF8.GetBytes("hello world");

            Assert.Equal("hello world".GetSHA256(), data.GetSHA256());
        }

        [Fact]
        public void GetSHA256_Object_ShouldHashSerializedObject()
        {
            var value = new { Id = 1, Name = "Test" };

            Assert.Equal("33ef42e63ba6307d7d1dc46e560250be5c6e022c3b274b6d55a7b46569dd3293", value.GetSHA256());
        }

        [Fact]
        public void GetSHA256_Stream_ShouldMatchByteArrayResult()
        {
            byte[] data = Encoding.UTF8.GetBytes("Hello, World!");

            using (var stream = new MemoryStream(data))
            {
                Assert.Equal(data.GetSHA256(), stream.GetSHA256());
            }
        }

        [Fact]
        public void GetHMACSHA256_String_ShouldMatchKnownVector()
        {
            string message = "The quick brown fox jumps over the lazy dog";

            Assert.Equal(
                "f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8",
                message.GetHMACSHA256("key"));
        }

        [Fact]
        public void GetHMACSHA256_ByteArray_ShouldMatchStringResult()
        {
            byte[] data = Encoding.UTF8.GetBytes("Hello, World!");
            byte[] key = Encoding.UTF8.GetBytes("randomSalt");

            Assert.Equal("Hello, World!".GetHMACSHA256("randomSalt"), data.GetHMACSHA256(key));
        }

        [Fact]
        public void GetHMACSHA256_Object_ShouldHashSerializedObject()
        {
            var value = new { Id = 1, Name = "Test" };

            Assert.Equal("78ab1e443cba886a54be7bf5386a9f26ddaca9e43778375a637488b26cc85ef2", value.GetHMACSHA256("randomSalt"));
        }

        [Fact]
        public void GetHMACSHA256_Stream_ShouldMatchByteArrayResult()
        {
            byte[] data = Encoding.UTF8.GetBytes("Hello, World!");
            byte[] key = Encoding.UTF8.GetBytes("randomSalt");

            using (var stream = new MemoryStream(data))
            {
                Assert.Equal(data.GetHMACSHA256(key), stream.GetHMACSHA256(key));
            }
        }

        [Fact]
        public void GetHMACSHA256_ShouldReturnNullForNullKey()
        {
            string? key = null;

            Assert.Null("hello world".GetHMACSHA256(key));
        }
    }
}
