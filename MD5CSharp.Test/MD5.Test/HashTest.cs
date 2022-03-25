using Xunit;
using MD5Hash;

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
    }
}