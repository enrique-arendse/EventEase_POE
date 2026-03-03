using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace EventEase_POE.Service
{
	public static class PasswordHasher
	{
        // HashPassword returns a string containing the salt and the derived key in the format: {saltBase64}:{hashBase64}
        public static string HashPassword(string password)
        {
            // Generate a 128-bit salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Derive a 256-bit subkey (hash)
            byte[] derived = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string saltB64 = Convert.ToBase64String(salt);
            string hashB64 = Convert.ToBase64String(derived);

            return $"{saltB64}:{hashB64}";
        }

        // VerifyPassword accepts the stored value produced by HashPassword and the plain password to verify.
        // It supports legacy values (no colon) by falling back to a simple comparison.
        public static bool VerifyPassword(string stored, string password)
        {
            if (string.IsNullOrEmpty(stored))
                return false;

            // New format: salt:hash
            if (stored.Contains(':'))
            {
                var parts = stored.Split(':');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                byte[] derived = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: storedHash.Length);

                // Constant time comparison
                if (derived.Length != storedHash.Length)
                    return false;

                int diff = 0;
                for (int i = 0; i < derived.Length; i++)
                    diff |= derived[i] ^ storedHash[i];

                return diff == 0;
            }

            // Legacy fallback: if stored value doesn't contain salt, allow direct comparison
            return stored == password;
        }
	}
}
