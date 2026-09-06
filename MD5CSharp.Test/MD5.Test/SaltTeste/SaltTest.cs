using MD5Hash;
using System.IO;
using System.Text;
using Xunit;

namespace MD5.Test
{
    public class SaltTest
    {
        [Fact]
        public void MD5HashGetMD5WithSalt_ShouldReturnDifferentHashesForSameInputWithDifferentSalts()
        {
            string input = "hello world";
            string salt1 = "randomSalt1";
            string salt2 = "randomSalt2";

            string hash1 = input.GetMD5WithSalt(salt1);
            string hash2 = input.GetMD5WithSalt(salt2);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ShouldReturnSameHashForSameInputAndSameSalt()
        {
            string input = "hello world";
            string salt = "randomSalt";

            string hash1 = input.GetMD5WithSalt(salt);
            string hash2 = input.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ShouldReturnNullForNullInput()
        {
            string? input = null;
            string salt = "randomSalt";

            string hash = input.GetMD5WithSalt(salt);

            Assert.Null(hash);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ShouldReturnDifferentHashesForDifferentInputsWithSameSalt()
        {
            string input1 = "hello world";
            string input2 = "goodbye world";
            string salt = "randomSalt";

            string hash1 = input1.GetMD5WithSalt(salt);
            string hash2 = input2.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_String_ShouldPreserveCurrentPrefixSaltResult()
        {
            string hash = "hello world".GetMD5WithSalt("randomSalt");

            Assert.Equal("c366e5ffe5fdb994846b13d36eda3595", hash);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Object_ShouldReturnDifferentHashesForDifferentObjectsWithSameSalt()
        {
            var obj1 = new { Id = 1, Name = "Test1" };
            var obj2 = new { Id = 2, Name = "Test2" };
            string salt = "randomSalt";

            string hash1 = obj1.GetMD5WithSalt(salt);
            string hash2 = obj2.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Object_ShouldReturnSameHashForSameObjectAndSameSalt()
        {
            var obj = new { Id = 1, Name = "Test" };
            string salt = "randomSalt";

            string hash1 = obj.GetMD5WithSalt(salt);
            string hash2 = obj.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Object_ShouldPreserveCurrentPrefixSaltResult()
        {
            var obj = new { Id = 1, Name = "Test" };

            string hash = obj.GetMD5WithSalt("randomSalt");

            Assert.Equal("47f05f41e2bc70a9c578c7d79c598724", hash);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ByteArray_ShouldReturnDifferentHashesForDifferentByteArraysWithSameSalt()
        {
            byte[] byteArray1 = Encoding.UTF8.GetBytes("Hello, World!");
            byte[] byteArray2 = Encoding.UTF8.GetBytes("Goodbye, World!");
            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");

            string hash1 = byteArray1.GetMD5WithSalt(salt);
            string hash2 = byteArray2.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ByteArray_ShouldReturnSameHashForSameByteArrayAndSameSalt()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Hello, World!");
            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");

            string hash1 = byteArray.GetMD5WithSalt(salt);
            string hash2 = byteArray.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_ByteArray_ShouldPreserveCurrentSuffixSaltResult()
        {
            byte[] byteArray = Encoding.UTF8.GetBytes("Hello, World!");
            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");

            string hash = byteArray.GetMD5WithSalt(salt);

            Assert.Equal("42e794363cd8641f7174e809e6cd2bc6", hash);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Stream_ShouldReturnDifferentHashesForDifferentStreamsWithSameSalt()
        {
            var stream1 = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            var stream2 = new MemoryStream(Encoding.UTF8.GetBytes("Goodbye, World!"));
            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");

            string hash1 = stream1.GetMD5WithSalt(salt);
            string hash2 = stream2.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Stream_ShouldReturnSameHashForSameStreamAndSameSalt()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, World!"));
            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");

            string hash1 = stream.GetMD5WithSalt(salt);
            stream.Position = 0;
            string hash2 = stream.GetMD5WithSalt(salt);

            Assert.NotNull(hash1);
            Assert.NotNull(hash2);
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void MD5HashGetMD5WithSalt_Stream_ShouldMatchByteArrayForLargeInput()
        {
            byte[] input = new byte[200000];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = (byte)(i % 251);
            }

            byte[] salt = Encoding.UTF8.GetBytes("randomSalt");
            string expectedHash = input.GetMD5WithSalt(salt);

            using (var stream = new MemoryStream(input))
            {
                string actualHash = stream.GetMD5WithSalt(salt);

                Assert.Equal(expectedHash, actualHash);
            }
        }

    }
}
