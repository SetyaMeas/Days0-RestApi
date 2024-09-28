using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;

namespace RestApi.Src.Utils
{
    public class PasswordHashing
    {
        public string Hash(string password)
        {
            byte[] salt = new byte[32];
            new Random().NextBytes(salt);

            var config = new Argon2Config
            {
                TimeCost = 10,
                MemoryCost = 32768, // 32MB
                Version = Argon2Version.Nineteen,
                Password = Encoding.UTF8.GetBytes(password),
                Salt = salt,
                HashLength = 20,
                Lanes = 5,
                Threads = Environment.ProcessorCount,
            };

            var argon2 = new Argon2(config);
            string hashedPassword;
            using (SecureArray<byte> hash = argon2.Hash())
            {
                hashedPassword = config.EncodeString(hash.Buffer);
            }
            return hashedPassword;
        }

        public bool Verify(string password, string hashedPassword)
        {
            return (Argon2.Verify(hashedPassword, password));
        }
    }
}
