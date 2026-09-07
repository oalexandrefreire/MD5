using MD5Hash;
using Xunit;

namespace MD5.Test
{
    public class PasswordHashTest
    {
        [Fact]
        public void HashPBKDF2_ShouldVerifyCorrectPasswordAndRejectWrongPassword()
        {
            string passwordHash = PasswordHash.HashPBKDF2("correct horse", 1000, 16, 32);

            Assert.StartsWith("pbkdf2-sha256$1000$", passwordHash);
            Assert.True(PasswordHash.VerifyPBKDF2("correct horse", passwordHash));
            Assert.False(PasswordHash.VerifyPBKDF2("wrong horse", passwordHash));
        }

        [Fact]
        public void HashPBKDF2_ShouldGenerateDifferentSaltForEachHash()
        {
            string firstHash = PasswordHash.HashPBKDF2("same password", 1000, 16, 32);
            string secondHash = PasswordHash.HashPBKDF2("same password", 1000, 16, 32);

            Assert.NotEqual(firstHash, secondHash);
            Assert.True(PasswordHash.VerifyPBKDF2("same password", firstHash));
            Assert.True(PasswordHash.VerifyPBKDF2("same password", secondHash));
        }

        [Fact]
        public void VerifyPBKDF2_ShouldMatchKnownHmacSha256Vector()
        {
            const string passwordHash =
                "pbkdf2-sha256$1$c2FsdDEyMzQ=$A3txk3Whu0pO3YU3bLiz5X8zoIv2nAJkJIgk9xLhUJ8=";

            Assert.True(PasswordHash.VerifyPBKDF2("password", passwordHash));
        }

        [Fact]
        public void HashBCrypt_ShouldVerifyCorrectPasswordAndRejectWrongPassword()
        {
            string passwordHash = PasswordHash.HashBCrypt("correct horse", 4);

            Assert.StartsWith("$2", passwordHash);
            Assert.True(PasswordHash.VerifyBCrypt("correct horse", passwordHash));
            Assert.False(PasswordHash.VerifyBCrypt("wrong horse", passwordHash));
        }

        [Fact]
        public void HashArgon2_ShouldVerifyCorrectPasswordAndRejectWrongPassword()
        {
            string passwordHash = PasswordHash.HashArgon2(
                "correct horse",
                iterations: 2,
                memorySize: 8192,
                degreeOfParallelism: 1,
                saltSize: 16,
                hashSize: 32);

            Assert.StartsWith("argon2id$v=19$m=8192,t=2,p=1$", passwordHash);
            Assert.True(PasswordHash.VerifyArgon2("correct horse", passwordHash));
            Assert.False(PasswordHash.VerifyArgon2("wrong horse", passwordHash));
        }

        [Fact]
        public void PasswordHashVerification_ShouldReturnFalseForMalformedHashes()
        {
            Assert.False(PasswordHash.VerifyPBKDF2("password", "not-a-pbkdf2-hash"));
            Assert.False(PasswordHash.VerifyBCrypt("password", "not-a-bcrypt-hash"));
            Assert.False(PasswordHash.VerifyArgon2("password", "not-an-argon2-hash"));
        }
    }
}
