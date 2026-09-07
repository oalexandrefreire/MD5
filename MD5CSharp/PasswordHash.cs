using Konscious.Security.Cryptography;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using BCryptAlgorithm = BCrypt.Net.BCrypt;

namespace MD5Hash
{
    /// <summary>
    /// Password-specific hashing functions. These APIs are independent from the
    /// legacy MD5 APIs exposed by <see cref="Hash"/>.
    /// </summary>
    public static class PasswordHash
    {
        private const int DefaultPbkdf2Iterations = 600000;
        private const int DefaultSaltSize = 16;
        private const int DefaultHashSize = 32;
        private const int DefaultBcryptWorkFactor = 12;
        private const int DefaultArgon2Iterations = 3;
        private const int DefaultArgon2MemorySize = 65536;
        private const int DefaultArgon2DegreeOfParallelism = 2;

        private const int MinimumSaltSize = 8;
        private const int MaximumEncodedValueSize = 1024;
        private const int MaximumPbkdf2Iterations = 100000000;
        private const int MaximumArgon2MemorySize = 1048576;
        private const int MaximumArgon2Iterations = 1000;
        private const int MaximumArgon2DegreeOfParallelism = 64;

        /// <summary>Creates a PBKDF2-HMAC-SHA256 password hash.</summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="iterations">The PBKDF2 iteration count. The default is 600,000.</param>
        /// <param name="saltSize">The generated salt size in bytes. The default is 16.</param>
        /// <param name="hashSize">The derived hash size in bytes. The default is 32.</param>
        /// <returns>A self-contained encoded hash containing the algorithm, parameters, salt and derived key.</returns>
        public static string HashPBKDF2(
            string password,
            int iterations = DefaultPbkdf2Iterations,
            int saltSize = DefaultSaltSize,
            int hashSize = DefaultHashSize)
        {
            RequirePassword(password);
            ValidatePbkdf2Parameters(iterations, saltSize, hashSize);

            byte[] salt = GenerateRandomBytes(saltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] derivedKey;

            try
            {
                derivedKey = DerivePbkdf2Sha256(passwordBytes, salt, iterations, hashSize);

                try
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "pbkdf2-sha256${0}${1}${2}",
                        iterations,
                        Convert.ToBase64String(salt),
                        Convert.ToBase64String(derivedKey));
                }
                finally
                {
                    Array.Clear(derivedKey, 0, derivedKey.Length);
                }
            }
            finally
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
            }
        }

        /// <summary>Verifies a password against a hash returned by <see cref="HashPBKDF2"/>.</summary>
        public static bool VerifyPBKDF2(string password, string passwordHash)
        {
            RequirePassword(password);
            RequireValue(passwordHash, nameof(passwordHash));

            string[] parts = passwordHash.Split('$');
            if (parts.Length != 4 || !string.Equals(parts[0], "pbkdf2-sha256", StringComparison.Ordinal))
                return false;

            try
            {
                int iterations = ParseInteger(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);

                ValidatePbkdf2Parameters(iterations, salt.Length, expectedHash.Length);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                try
                {
                    byte[] actualHash = DerivePbkdf2Sha256(passwordBytes, salt, iterations, expectedHash.Length);
                    try
                    {
                        return FixedTimeEquals(actualHash, expectedHash);
                    }
                    finally
                    {
                        Array.Clear(actualHash, 0, actualHash.Length);
                    }
                }
                finally
                {
                    Array.Clear(passwordBytes, 0, passwordBytes.Length);
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>Creates a bcrypt password hash. The work factor is embedded in the result.</summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="workFactor">The bcrypt cost factor. The default is 12.</param>
        public static string HashBCrypt(string password, int workFactor = DefaultBcryptWorkFactor)
        {
            RequirePassword(password);
            if (workFactor < 4 || workFactor > 31)
                throw new ArgumentOutOfRangeException(nameof(workFactor), "The bcrypt work factor must be between 4 and 31.");

            return BCryptAlgorithm.HashPassword(password, workFactor);
        }

        /// <summary>Verifies a password against a bcrypt hash returned by <see cref="HashBCrypt"/>.</summary>
        public static bool VerifyBCrypt(string password, string passwordHash)
        {
            RequirePassword(password);
            RequireValue(passwordHash, nameof(passwordHash));

            try
            {
                return BCryptAlgorithm.Verify(password, passwordHash);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }

        /// <summary>Creates an Argon2id password hash.</summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="iterations">The Argon2 iteration count. The default is 3.</param>
        /// <param name="memorySize">The memory size in KiB. The default is 65,536 KiB.</param>
        /// <param name="degreeOfParallelism">The number of Argon2 lanes. The default is 2.</param>
        /// <param name="saltSize">The generated salt size in bytes. The default is 16.</param>
        /// <param name="hashSize">The derived hash size in bytes. The default is 32.</param>
        public static string HashArgon2(
            string password,
            int iterations = DefaultArgon2Iterations,
            int memorySize = DefaultArgon2MemorySize,
            int degreeOfParallelism = DefaultArgon2DegreeOfParallelism,
            int saltSize = DefaultSaltSize,
            int hashSize = DefaultHashSize)
        {
            RequirePassword(password);
            ValidateArgon2Parameters(iterations, memorySize, degreeOfParallelism, saltSize, hashSize);

            byte[] salt = GenerateRandomBytes(saltSize);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] derivedKey;

            try
            {
                using (var argon2 = new Argon2id(passwordBytes))
                {
                    argon2.Salt = salt;
                    argon2.Iterations = iterations;
                    argon2.MemorySize = memorySize;
                    argon2.DegreeOfParallelism = degreeOfParallelism;
                    derivedKey = argon2.GetBytes(hashSize);
                }

                try
                {
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "argon2id$v=19$m={0},t={1},p={2}${3}${4}",
                        memorySize,
                        iterations,
                        degreeOfParallelism,
                        Convert.ToBase64String(salt),
                        Convert.ToBase64String(derivedKey));
                }
                finally
                {
                    Array.Clear(derivedKey, 0, derivedKey.Length);
                }
            }
            finally
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
            }
        }

        /// <summary>Verifies a password against an Argon2id hash returned by <see cref="HashArgon2"/>.</summary>
        public static bool VerifyArgon2(string password, string passwordHash)
        {
            RequirePassword(password);
            RequireValue(passwordHash, nameof(passwordHash));

            string[] parts = passwordHash.Split('$');
            if (parts.Length != 5 || !string.Equals(parts[0], "argon2id", StringComparison.Ordinal) ||
                !string.Equals(parts[1], "v=19", StringComparison.Ordinal))
                return false;

            try
            {
                int memorySize;
                int iterations;
                int degreeOfParallelism;
                ParseArgon2Parameters(parts[2], out memorySize, out iterations, out degreeOfParallelism);

                byte[] salt = Convert.FromBase64String(parts[3]);
                byte[] expectedHash = Convert.FromBase64String(parts[4]);
                ValidateArgon2Parameters(iterations, memorySize, degreeOfParallelism, salt.Length, expectedHash.Length);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                try
                {
                    using (var argon2 = new Argon2id(passwordBytes))
                    {
                        argon2.Salt = salt;
                        argon2.Iterations = iterations;
                        argon2.MemorySize = memorySize;
                        argon2.DegreeOfParallelism = degreeOfParallelism;

                        byte[] actualHash = argon2.GetBytes(expectedHash.Length);
                        try
                        {
                            return FixedTimeEquals(actualHash, expectedHash);
                        }
                        finally
                        {
                            Array.Clear(actualHash, 0, actualHash.Length);
                        }
                    }
                }
                finally
                {
                    Array.Clear(passwordBytes, 0, passwordBytes.Length);
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static void RequirePassword(string password)
        {
            RequireValue(password, nameof(password));
        }

        private static void RequireValue(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        private static byte[] GenerateRandomBytes(int size)
        {
            byte[] bytes = new byte[size];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(bytes);
            }

            return bytes;
        }

        private static byte[] DerivePbkdf2Sha256(byte[] password, byte[] salt, int iterations, int outputLength)
        {
            const int hashLength = 32;
            int blockCount = (outputLength + hashLength - 1) / hashLength;
            byte[] result = new byte[outputLength];
            byte[] saltAndCounter = new byte[salt.Length + 4];
            Buffer.BlockCopy(salt, 0, saltAndCounter, 0, salt.Length);

            using (var hmac = new HMACSHA256(password))
            {
                for (int block = 1; block <= blockCount; block++)
                {
                    saltAndCounter[salt.Length] = (byte)(block >> 24);
                    saltAndCounter[salt.Length + 1] = (byte)(block >> 16);
                    saltAndCounter[salt.Length + 2] = (byte)(block >> 8);
                    saltAndCounter[salt.Length + 3] = (byte)block;

                    byte[] current = hmac.ComputeHash(saltAndCounter);
                    byte[] accumulated = new byte[hashLength];
                    Buffer.BlockCopy(current, 0, accumulated, 0, hashLength);

                    try
                    {
                        for (int iteration = 1; iteration < iterations; iteration++)
                        {
                            byte[] next = hmac.ComputeHash(current);
                            Array.Clear(current, 0, current.Length);
                            current = next;
                            for (int i = 0; i < hashLength; i++)
                                accumulated[i] ^= current[i];
                        }

                        int offset = (block - 1) * hashLength;
                        int bytesToCopy = Math.Min(hashLength, outputLength - offset);
                        Buffer.BlockCopy(accumulated, 0, result, offset, bytesToCopy);
                    }
                    finally
                    {
                        Array.Clear(current, 0, current.Length);
                        Array.Clear(accumulated, 0, accumulated.Length);
                    }
                }
            }

            Array.Clear(saltAndCounter, 0, saltAndCounter.Length);
            return result;
        }

        private static void ValidatePbkdf2Parameters(int iterations, int saltSize, int hashSize)
        {
            if (iterations < 1 || iterations > MaximumPbkdf2Iterations)
                throw new ArgumentOutOfRangeException(nameof(iterations), "The PBKDF2 iteration count is outside the supported range.");
            if (saltSize < MinimumSaltSize || saltSize > MaximumEncodedValueSize)
                throw new ArgumentOutOfRangeException(nameof(saltSize), "The salt size must be between 8 and 1024 bytes.");
            if (hashSize < 1 || hashSize > MaximumEncodedValueSize)
                throw new ArgumentOutOfRangeException(nameof(hashSize), "The hash size must be between 1 and 1024 bytes.");
        }

        private static void ValidateArgon2Parameters(int iterations, int memorySize, int degreeOfParallelism, int saltSize, int hashSize)
        {
            if (iterations < 1 || iterations > MaximumArgon2Iterations)
                throw new ArgumentOutOfRangeException(nameof(iterations), "The Argon2 iteration count is outside the supported range.");
            if (memorySize < 8 || memorySize > MaximumArgon2MemorySize)
                throw new ArgumentOutOfRangeException(nameof(memorySize), "The Argon2 memory size must be between 8 and 1,048,576 KiB.");
            if (degreeOfParallelism < 1 || degreeOfParallelism > MaximumArgon2DegreeOfParallelism)
                throw new ArgumentOutOfRangeException(nameof(degreeOfParallelism), "The Argon2 degree of parallelism must be between 1 and 64.");
            if (memorySize < degreeOfParallelism * 8)
                throw new ArgumentException("The Argon2 memory size must support at least 8 KiB per lane.", nameof(memorySize));
            if (saltSize < MinimumSaltSize || saltSize > MaximumEncodedValueSize)
                throw new ArgumentOutOfRangeException(nameof(saltSize), "The salt size must be between 8 and 1024 bytes.");
            if (hashSize < 1 || hashSize > MaximumEncodedValueSize)
                throw new ArgumentOutOfRangeException(nameof(hashSize), "The hash size must be between 1 and 1024 bytes.");
        }

        private static int ParseInteger(string value)
        {
            return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
        }

        private static void ParseArgon2Parameters(string value, out int memorySize, out int iterations, out int degreeOfParallelism)
        {
            memorySize = 0;
            iterations = 0;
            degreeOfParallelism = 0;

            string[] parameters = value.Split(',');
            if (parameters.Length != 3)
                throw new FormatException("Invalid Argon2 parameters.");

            foreach (string parameter in parameters)
            {
                string[] pair = parameter.Split('=');
                if (pair.Length != 2)
                    throw new FormatException("Invalid Argon2 parameter.");

                int parsedValue = ParseInteger(pair[1]);
                switch (pair[0])
                {
                    case "m":
                        memorySize = parsedValue;
                        break;
                    case "t":
                        iterations = parsedValue;
                        break;
                    case "p":
                        degreeOfParallelism = parsedValue;
                        break;
                    default:
                        throw new FormatException("Unknown Argon2 parameter.");
                }
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            int difference = left.Length ^ right.Length;
            int comparisonLength = Math.Max(left.Length, right.Length);

            for (int i = 0; i < comparisonLength; i++)
            {
                byte leftByte = i < left.Length ? left[i] : (byte)0;
                byte rightByte = i < right.Length ? right[i] : (byte)0;
                difference |= leftByte ^ rightByte;
            }

            return difference == 0;
        }
    }
}
