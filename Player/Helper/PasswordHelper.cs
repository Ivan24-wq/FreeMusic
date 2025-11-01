using System.Security.Cryptography;
using System.Text;
namespace Player.Helper
{
    public static class PasswordHelper
    {
        //Генерация уникальной строки для каждого пользователя
        public static string GenaretionSalt(int size = 16)
        {
            byte[] saltByte = new byte[size];
            //Генерация уникальной строки
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltByte);
            return Convert.ToBase64String(saltByte);
        }

        // Хэширование пароля
        public static string PasswordHashed(string password, string salt)
        {
            //Используем алгоритм sha256
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        //Проверка совпадает ли пароль с введённым
        public static bool Verification(string enteredPassword, string storeHash, string storeSalt)
        {
            var hashOfInput = PasswordHashed(enteredPassword, storeSalt);
            return hashOfInput == storeHash;
        }
    }
}