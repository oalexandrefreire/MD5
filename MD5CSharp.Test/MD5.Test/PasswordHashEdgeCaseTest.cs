using System;
using System.Text;
using MD5Hash;
using Xunit;

namespace MD5.Test
{
    public class PasswordHashEdgeCaseTest
    {
        [Fact]
        public void PasswordHash_ShouldRejectNullRequiredValues()
        {
            Assert.Throws<ArgumentNullException>(() => PasswordHash.HashPBKDF2(null, 1, 8, 1));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyPBKDF2(null, "hash"));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyPBKDF2("password", null));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.HashBCrypt(null, 4));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyBCrypt(null, "hash"));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyBCrypt("password", null));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.HashArgon2(null, 1, 16, 1, 8, 1));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyArgon2(null, "hash"));
            Assert.Throws<ArgumentNullException>(() => PasswordHash.VerifyArgon2("password", null));
        }

        [Fact]
        public void HashPBKDF2_ShouldValidateAllParameters()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 0, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 100000001, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 1, 7, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 1, 1025, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 1, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashPBKDF2("password", 1, 8, 1025));
        }

        [Fact]
        public void HashPBKDF2_ShouldSupportMultipleBlocksAndPartialFinalBlock()
        {
            string passwordHash = PasswordHash.HashPBKDF2("password", 2, 8, 33);

            Assert.True(PasswordHash.VerifyPBKDF2("password", passwordHash));
            Assert.False(PasswordHash.VerifyPBKDF2("wrong", passwordHash));
        }

        [Fact]
        public void VerifyPBKDF2_ShouldReturnFalseForEveryMalformedInputCategory()
        {
            string validSalt = Convert.ToBase64String(Encoding.UTF8.GetBytes("12345678"));
            string validHash = Convert.ToBase64String(new byte[] { 1 });

            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$abc$" + validSalt + "$" + validHash));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$999999999999999999999$" + validSalt + "$" + validHash));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$1$not-base64$" + validHash));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$1$" + validSalt + "$not-base64"));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$1$" + Convert.ToBase64String(new byte[7]) + "$" + validHash));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$1$" + validSalt + "$"));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "pbkdf2-sha256$1$" + validSalt + "$" + validHash + "$extra"));
            Assert.False(PasswordHash.VerifyPBKDF2("password", "other$1$" + validSalt + "$" + validHash));
        }

        [Fact]
        public void HashBCrypt_ShouldValidateWorkFactorAndMalformedHashes()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashBCrypt("password", 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashBCrypt("password", 32));
            Assert.False(PasswordHash.VerifyBCrypt("password", "$2b$04$invalid"));
        }

        [Fact]
        public void HashArgon2_ShouldValidateAllParameters()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 0, 16, 1, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1001, 16, 1, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 7, 1, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 1048577, 1, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 0, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 65, 8, 1));
            Assert.Throws<ArgumentException>(() => PasswordHash.HashArgon2("password", 1, 8, 2, 8, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 1, 7, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 1, 1025, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 1, 8, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => PasswordHash.HashArgon2("password", 1, 16, 1, 8, 1025));
        }

        [Fact]
        public void VerifyArgon2_ShouldReturnFalseForEveryMalformedInputCategory()
        {
            string salt = Convert.ToBase64String(Encoding.UTF8.GetBytes("12345678"));
            string hash = Convert.ToBase64String(new byte[] { 1 });
            string prefix = "argon2id$v=19$";

            Assert.False(PasswordHash.VerifyArgon2("password", "not-argon2"));
            Assert.False(PasswordHash.VerifyArgon2("password", "argon2id$v=18$m=16,t=1,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=1,q=2$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "x=16,t=1,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=abc,t=1,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=999999999999999999999,t=1,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=1$not-base64$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=1$" + salt + "$not-base64"));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=1$" + Convert.ToBase64String(new byte[7]) + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=8,t=1,p=2$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=1048577,t=1,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=0,p=1$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=65$" + salt + "$" + hash));
            Assert.False(PasswordHash.VerifyArgon2("password", prefix + "m=16,t=1,p=1$" + salt + "$$"));
        }
    }
}
