using System.Security.Cryptography;
using System.Text;

namespace Domain;

public class Settings
{
    public static char[] UkrainianAlphabet = new char[]
    {
        'А', 'Б', 'В', 'Г', 'Ґ', 'Д', 'Е', 'Є', 'Ж', 'З',
        'И', 'І', 'Ї', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П',
        'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ',
        'Ь', 'Ю', 'Я', '.', '_', '-', ' ', '#'
    };
    public static byte[] HashMessageSHA1(string message)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            // Перетворюємо повідомлення в байти
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Обчислюємо SHA-1 хеш повідомлення
            byte[] hash = sha1.ComputeHash(messageBytes);

            // щоб отримати рівно 192 біти, то розширюємо до 24 байт
            byte[] truncatedHash = new byte[24];

            Array.Copy(hash, truncatedHash, 20); // Перших 20 байт з SHA-1
            // Останні 4 байти доповнюються нулями, щоб досягти 192 бітів
            return truncatedHash;
        }
    }
}