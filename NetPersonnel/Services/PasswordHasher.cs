using System.Security.Cryptography;

namespace NetPersonnel.Services
{
    public class PasswordHasher
    {

        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt, int iterations = 100_000)
        {
            using var rng = RandomNumberGenerator.Create();
            salt = new byte[64];
            rng.GetBytes(salt);

            using var derive = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            hash = derive.GetBytes(64);
        }


        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt, int iterations = 100_000)
        {
            using var derive = new Rfc2898DeriveBytes(password, storedSalt, iterations, HashAlgorithmName.SHA256);
            var computed = derive.GetBytes(storedHash.Length);
            return CryptographicOperations.FixedTimeEquals(computed, storedHash);
        }
    }
}
