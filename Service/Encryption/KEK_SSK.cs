using Domain;
using Domain.Attributes;
using Domain.IService.IEncryption;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Service.Encryption.ECCService;

namespace Service.Encryption
{
    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IKEK_SSK))]
    public class KEK_SSK : IKEK_SSK
    {
        private byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private string ByteArrayToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        public EncryptionModel Encryption(EncryptionModel model)
        {
            //Мастер-ключ из HEX в байты
            byte[] masterKey = HexStringToByteArray(model.PrimeP);

            //Генерация сеансового ключа (16 байт)
            byte[] sessionKey = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(sessionKey);
            }

            //Шифруем сеансовый ключ мастер-ключом (3DES)
            byte[] encryptedSessionKey;
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = masterKey;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var encryptor = tdes.CreateEncryptor())
                {
                    encryptedSessionKey = encryptor.TransformFinalBlock(sessionKey, 0, sessionKey.Length);
                }
            }

            //Шифруем текст сеансовым ключом (AES)
            byte[] encryptedMessage;
            using (var aes = Aes.Create())
            {
                aes.Key = sessionKey;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV(); // Вектор инициализации

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(model.FileText);
                    encryptedMessage = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    // Добавляем IV в начало зашифрованного текста
                    byte[] combined = new byte[aes.IV.Length + encryptedMessage.Length];
                    Array.Copy(aes.IV, 0, combined, 0, aes.IV.Length);
                    Array.Copy(encryptedMessage, 0, combined, aes.IV.Length, encryptedMessage.Length);
                    encryptedMessage = combined;
                }
            }

            return new EncryptionModel
            {
                FileText = ByteArrayToHexString(encryptedMessage),            // Зашифрованный текст
                CloseKeyD = ByteArrayToHexString(encryptedSessionKey) // Зашифрованный сеансовый ключ
            };
        }
        public EncryptionModel Decryption(EncryptionModel model)
        {

            //Преобразуем мастер-ключ и зашифрованный сеансовый ключ из HEX
            byte[] masterKey = HexStringToByteArray(model.PrimeP);
            byte[] encryptedSessionKey = HexStringToByteArray(model.CloseKeyD);

            //Дешифруем сеансовый ключ мастер-ключом (3DES)
            byte[] sessionKey;
            using (var tdes = new TripleDESCryptoServiceProvider())
            {
                tdes.Key = masterKey;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                using (var decryptor = tdes.CreateDecryptor())
                {
                    sessionKey = decryptor.TransformFinalBlock(encryptedSessionKey, 0, encryptedSessionKey.Length);
                }
            }

            //Преобразуем зашифрованный текст из HEX
            byte[] encryptedMessage = HexStringToByteArray(model.FileText);

            //  Извлекаем IV (первые 16 байт для AES)
            byte[] iv = new byte[16];
            byte[] cipherText = new byte[encryptedMessage.Length - 16];
            Array.Copy(encryptedMessage, 0, iv, 0, 16);
            Array.Copy(encryptedMessage, 16, cipherText, 0, cipherText.Length);

            // Дешифруем сообщение сеансовым ключом (AES)
            string decryptedText;
            using (var aes = Aes.Create())
            {
                aes.Key = sessionKey;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                }
            }

            return new EncryptionModel
            {
                FileText = decryptedText
            };
        }
    }
}
