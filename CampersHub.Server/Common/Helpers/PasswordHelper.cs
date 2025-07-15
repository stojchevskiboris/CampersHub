using System.Text;
using System.Security.Cryptography;

namespace CampersHub.Server.Common.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = new SHA256Managed())
            {
                //var salt = GenerateSalt();
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                //byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

                // Concatenate password and salt
                //Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                //Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

                // Hash the concatenated password and salt
                byte[] hashedBytes = sha256.ComputeHash(passwordBytes);

                // Concatenate the salt and hashed password for storage
                //byte[] hashedPasswordWithSalt = new byte[hashedBytes.Length + salt.Length];
                //Buffer.BlockCopy(salt, 0, hashedPasswordWithSalt, 0, salt.Length);
                //Buffer.BlockCopy(hashedBytes, 0, hashedPasswordWithSalt, salt.Length, hashedBytes.Length);

                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static byte[] GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16]; // Adjust the size based on your security requirements
                rng.GetBytes(salt);
                return salt;
            }
        }

        public static bool CheckPasswordStrength(string password)
        {
            // Minimum length requirement
            if (password.Length < 8)
                return false;

            // Check for at least one uppercase letter
            //if (!password.Any(char.IsUpper))
            //    return false;

            // Check for at least one lowercase letter
            //if (!password.Any(char.IsLower))
            //    return false;

            // Check for at least one digit
            //if (!password.Any(char.IsDigit))
            //    return false;

            // Check for at least one special character (non-alphanumeric)
            //if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            //    return false;

            // If all checks pass, the password is strong
            return true;
        }

        public static string DecryptString(string cipherText)
        {
            Aes aes = GetEncryptionAlgorithm();
            byte[] buffer = Convert.FromBase64String(cipherText);
            MemoryStream memoryStream = new MemoryStream(buffer);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            StreamReader streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }

        private static Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            var secret_key = Encoding.UTF8.GetBytes("1203199320052021");
            var initialization_vector = Encoding.UTF8.GetBytes("1203199320052021");
            aes.Key = secret_key;
            aes.IV = initialization_vector;
            return aes;
        }
    }
}
