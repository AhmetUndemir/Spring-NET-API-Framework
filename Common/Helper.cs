using System.Text;

namespace SimpleSprint.Common {
    public static class Helper {
        public static void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash (Encoding.UTF8.GetBytes (password));
            }

        }

    }
}