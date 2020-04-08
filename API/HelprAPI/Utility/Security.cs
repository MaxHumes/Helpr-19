using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace HelprAPI.Utility
{
    public static class Security
    {
        // generate a 32 byte long salt using a secure PRNG
        private static int saltLengthLimit = 32;
        public static byte[] GetSalt()
        {
            return GetSalt(saltLengthLimit);
        }
        //generate salt of length maximumSaltLength
        public static byte[] GetSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
        //generates a salted and hashed string of 128 bits
        public static string HashPassword(string password, byte[] salt)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 128 / 8));

            return hashed;
        }
    }
}
