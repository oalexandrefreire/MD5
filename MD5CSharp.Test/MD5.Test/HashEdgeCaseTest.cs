using System;
using System.IO;
using System.Text;
using MD5Hash;
using Xunit;

namespace MD5.Test
{
    public class HashEdgeCaseTest
    {
        [Fact]
        public void HashMethods_ShouldSupportEveryEncodingType()
        {
            foreach (EncodingType encodingType in Enum.GetValues(typeof(EncodingType)))
            {
                Assert.NotNull(Hash.Content("café", encodingType));
                Assert.NotNull("café".GetSHA256(encodingType));
                Assert.NotNull("café".GetHMACSHA256("key", encodingType));
                Assert.NotNull("café".GetMD5WithSalt("salt", encodingType));
            }
        }

        [Fact]
        public void HashMethods_ShouldReturnNullForUnsupportedEncoding()
        {
            EncodingType unsupported = (EncodingType)999;

            Assert.Null(Hash.Content("text", unsupported));
            Assert.Null("text".GetMD5(unsupported));
            Assert.Null("text".GetSHA256(unsupported));
            Assert.Null("text".GetHMACSHA256("key", unsupported));
            Assert.Null("text".GetMD5WithSalt("salt", unsupported));
        }

        [Fact]
        public void HashMethods_ShouldReturnNullForNullInputsWhereSupported()
        {
            Assert.Null(((byte[])null!).GetSHA256());
            Assert.Null(((Stream)null!).GetSHA256());
            Assert.Null(((byte[])null!).GetHMACSHA256(new byte[] { 1 }));
            Assert.Null(new byte[] { 1 }.GetHMACSHA256(null!));
            Assert.Null(((Stream)null!).GetHMACSHA256(new byte[] { 1 }));
            Assert.Null(new MemoryStream().GetHMACSHA256(null!));
            Assert.Null(((string)null!).GetSHA256());
            Assert.Null(((string)null!).GetHMACSHA256("key"));
            Assert.Null("text".GetHMACSHA256(null!));
            Assert.Null("".GetMD5WithSalt("salt"));
            Assert.Null(((byte[])null!).GetMD5WithSalt(new byte[] { 1 }));
            Assert.Null(new byte[] { 1 }.GetMD5WithSalt(null!));
        }

        [Fact]
        public void HashObjectMethods_ShouldReturnNullWhenSerializationFails()
        {
            var value = new ThrowingJsonValue();

            Assert.Null(value.GetMD5());
            Assert.Null(value.GetSHA256());
            Assert.Null(value.GetHMACSHA256(null!));
            Assert.Null(value.GetHMACSHA256("key"));
            Assert.Null(value.GetMD5WithSalt("salt"));
        }

        [Fact]
        public void GetMD5WithSalt_Stream_ShouldReturnNullWhenStreamOrSaltFails()
        {
            Assert.Null(((Stream)null!).GetMD5WithSalt(new byte[] { 1 }));
            Assert.Null(new MemoryStream(new byte[] { 1 }).GetMD5WithSalt(null!));

            using (var stream = new ThrowingStream())
            {
                Assert.Null(stream.GetMD5WithSalt(new byte[] { 1 }));
            }
        }

        private sealed class ThrowingJsonValue
        {
            public string Value
            {
                get { throw new InvalidOperationException("serialization failure"); }
            }
        }

        private sealed class ThrowingStream : MemoryStream
        {
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new IOException("read failure");
            }
        }
    }
}
