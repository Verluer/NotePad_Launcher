
using Domain.Attributes;
using Domain.IService.IEncryption; 
using Domain.Model; 
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO; 
using System.Security.Cryptography; 
using System.Text; 
using System.Text.Json; 
using System.Security.Cryptography.X509Certificates; 
using System.Collections.Generic;

namespace Service.Encryption 
{

    public static class RSAHelper 
    {
        public static RSA LoadRsaFromPem(string pemPath)
        {
            if (string.IsNullOrEmpty(pemPath)) throw new ArgumentNullException(nameof(pemPath));
            string pem = File.ReadAllText(pemPath);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(pem.ToCharArray());
            return rsa;
        }

        public static byte[] WrapKeyOAEP(RSA recipientPublicKey, byte[] key)
        {
            return recipientPublicKey.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] UnwrapKeyOAEP(RSA recipientPrivateKey, byte[] wrapped)
        {
            return recipientPrivateKey.Decrypt(wrapped, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] SignRsaPss(RSA privateKey, byte[] data)
        {
            return privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        }

        public static bool VerifyRsaPss(RSA publicKey, byte[] data, byte[] signature)
        {
            return publicKey.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        }
    } 

    [RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IAESService))] 
    public class AESService : IAESService, IDisposable 
    { 
        private readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        public EncryptionModel Encryption(EncryptionModel model, string recipientPublicPemPath, string signerPrivatePemPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (model.FileText == null) throw new ArgumentNullException(nameof(model.FileText));
            if (string.IsNullOrEmpty(recipientPublicPemPath)) throw new ArgumentNullException(nameof(recipientPublicPemPath));
            if (string.IsNullOrEmpty(signerPrivatePemPath)) throw new ArgumentNullException(nameof(signerPrivatePemPath));

            using var recipientPublic = RSAHelper.LoadRsaFromPem(recipientPublicPemPath);
            using var signerPrivate = RSAHelper.LoadRsaFromPem(signerPrivatePemPath);

            var plainBytes = Encoding.UTF8.GetBytes(model.FileText);

            var aesKeyBits = 128;
            int keyLen = aesKeyBits / 8;
            byte[] key = new byte[keyLen];
            byte[] iv = new byte[16];
            rng.GetBytes(iv);

            byte[] salt = null;
            int iterations = 0;
            var password = model.PrimeP; 
            if (!string.IsNullOrEmpty(password))
            {
                salt = new byte[16];
                rng.GetBytes(salt);
                iterations = 100_000;
                using var kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
                key = kdf.GetBytes(keyLen);
            }
            else
            {
                rng.GetBytes(key); 
            }

            byte[] cipherBytes = new byte[plainBytes.Length];
            using (var aes = Aes.Create())
            {
                aes.KeySize = aesKeyBits;
                aes.Mode = CipherMode.ECB; 
                aes.Padding = PaddingMode.None;
                aes.Key = key;

                byte[] feedback = (byte[])iv.Clone();
                byte[] outputBlock = new byte[16];

                using var encryptor = aes.CreateEncryptor();
                for (int i = 0; i < plainBytes.Length; i += 16)
                {
                    encryptor.TransformBlock(feedback, 0, 16, outputBlock, 0);
                    int blockLen = Math.Min(16, plainBytes.Length - i);
                    for (int j = 0; j < blockLen; j++)
                        cipherBytes[i + j] = (byte)(plainBytes[i + j] ^ outputBlock[j]);
                    Buffer.BlockCopy(outputBlock, 0, feedback, 0, 16);
                }
            }

            byte[] wrappedKey = null;
            if (string.IsNullOrEmpty(password))
            {
                wrappedKey = RSAHelper.WrapKeyOAEP(recipientPublic, key);
            }

            var meta = new EncryptionMetadata
            {
                Algorithm = "AES",
                KeySize = aesKeyBits,
                Mode = "OFB",
                IVHex = BytesToHex(iv),
                SaltHex = salt != null ? BytesToHex(salt) : null,
                Iterations = iterations,
                WrappedKeyHex = wrappedKey != null ? BytesToHex(wrappedKey) : null,
                KeyWrapAlgorithm = wrappedKey != null ? "RSA-OAEP-SHA256" : null,
                SignatureAlgorithm = "RSA-PSS-SHA256",
                PlaintextLength = plainBytes.Length
            };

            var metaForSigning = new
            {
                meta.Algorithm,
                meta.KeySize,
                meta.Mode,
                meta.IVHex,
                meta.SaltHex,
                meta.Iterations,
                meta.WrappedKeyHex,
                meta.KeyWrapAlgorithm,
                meta.SignatureAlgorithm,
                meta.PlaintextLength
            };
            var metaJson = JsonSerializer.Serialize(metaForSigning, new JsonSerializerOptions { WriteIndented = false });
            byte[] metaBytes = Encoding.UTF8.GetBytes(metaJson);

            byte[] payload = new byte[metaBytes.Length + cipherBytes.Length];
            Buffer.BlockCopy(metaBytes, 0, payload, 0, metaBytes.Length);
            Buffer.BlockCopy(cipherBytes, 0, payload, metaBytes.Length, cipherBytes.Length);

            byte[] signature = RSAHelper.SignRsaPss(signerPrivate, payload);
            meta.SignatureHex = BytesToHex(signature);

            var result = new EncryptionModel
            {
                FileText = BytesToHex(cipherBytes),
                Metadata = meta
            };

            ClearBytes(key);

            return result;
        }

        public EncryptionModel Decryption(EncryptionModel model, string recipientPrivatePemPath, string signerPublicPemPath)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(recipientPrivatePemPath)) throw new ArgumentNullException(nameof(recipientPrivatePemPath));
            if (string.IsNullOrEmpty(signerPublicPemPath)) throw new ArgumentNullException(nameof(signerPublicPemPath));
            if (string.IsNullOrEmpty(model.FileText)) throw new ArgumentNullException(nameof(model.FileText));
            if (model.Metadata == null) throw new ArgumentException("Missing metadata");

            using var recipientPrivate = RSAHelper.LoadRsaFromPem(recipientPrivatePemPath);
            using var signerPublic = RSAHelper.LoadRsaFromPem(signerPublicPemPath);

            var meta = model.Metadata;
            byte[] cipherBytes = HexToBytes(model.FileText);

            var metaForSigning = new
            {
                meta.Algorithm,
                meta.KeySize,
                meta.Mode,
                meta.IVHex,
                meta.SaltHex,
                meta.Iterations,
                meta.WrappedKeyHex,
                meta.KeyWrapAlgorithm,
                meta.SignatureAlgorithm,
                meta.PlaintextLength
            };
            var metaJson = JsonSerializer.Serialize(metaForSigning, new JsonSerializerOptions { WriteIndented = false });
            byte[] metaBytes = Encoding.UTF8.GetBytes(metaJson);

            byte[] payload = new byte[metaBytes.Length + cipherBytes.Length];
            Buffer.BlockCopy(metaBytes, 0, payload, 0, metaBytes.Length);
            Buffer.BlockCopy(cipherBytes, 0, payload, metaBytes.Length, cipherBytes.Length);

            if (string.IsNullOrEmpty(meta.SignatureHex)) throw new CryptographicException("Signature missing in metadata");
            byte[] signature = HexToBytes(meta.SignatureHex);

            if (!RSAHelper.VerifyRsaPss(signerPublic, payload, signature))
                throw new CryptographicException("Signature verification failed — file altered or wrong signer key!");


            byte[] iv = HexToBytes(meta.IVHex);

            byte[] key;
            if (!string.IsNullOrEmpty(model.PrimeP) && !string.IsNullOrEmpty(meta.SaltHex))
            {
                byte[] salt = HexToBytes(meta.SaltHex);
                int iterations = meta.Iterations;
                using var kdf = new Rfc2898DeriveBytes(model.PrimeP, salt, iterations, HashAlgorithmName.SHA256);
                key = kdf.GetBytes(meta.KeySize / 8);
            }
            else
            {
                if (string.IsNullOrEmpty(meta.WrappedKeyHex)) throw new CryptographicException("Wrapped key missing");
                byte[] wrappedKey = HexToBytes(meta.WrappedKeyHex);
                key = RSAHelper.UnwrapKeyOAEP(recipientPrivate, wrappedKey);
            }

            byte[] plainBytes = new byte[cipherBytes.Length];
            using (var aes = Aes.Create())
            {
                aes.KeySize = meta.KeySize;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                aes.Key = key;

                byte[] feedback = (byte[])iv.Clone();
                byte[] outputBlock = new byte[16];

                using var encryptor = aes.CreateEncryptor();
                for (int i = 0; i < cipherBytes.Length; i += 16)
                {
                    encryptor.TransformBlock(feedback, 0, 16, outputBlock, 0);
                    int blockLen = Math.Min(16, cipherBytes.Length - i);
                    for (int j = 0; j < blockLen; j++)
                        plainBytes[i + j] = (byte)(cipherBytes[i + j] ^ outputBlock[j]);
                    Buffer.BlockCopy(outputBlock, 0, feedback, 0, 16);
                }
            }

            long origLen = meta.PlaintextLength;
            if (origLen <= 0 || origLen > plainBytes.Length) origLen = plainBytes.Length;
            byte[] trimmed = new byte[origLen];
            Array.Copy(plainBytes, trimmed, origLen);

            var result = new EncryptionModel
            {
                FileText = Encoding.UTF8.GetString(trimmed)
            };

            ClearBytes(key);
            return result;
        }

        public void SaveEncryptedBundle(string path, string nameBase, EncryptionModel result) 
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(result.Metadata, options);
            File.WriteAllText($"{Path.Combine(path, $"{nameBase}_MetaData.json")}", json); 
            File.WriteAllText($"{Path.Combine(path, $"{nameBase}_Encrypted.txt")}", result.FileText);
        } 

        public EncryptionMetadata LoadEncryptedBundle(string path)
        {
            var json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<EncryptionMetadata>(json);
        }

        private static string BytesToHex(byte[] data) 
        {
            if (data == null || data.Length == 0) return string.Empty;
            var sb = new StringBuilder(data.Length * 2);
            foreach (var b in data) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        } 

        private static byte[] HexToBytes(string hex) 
        {
            if (string.IsNullOrEmpty(hex)) return Array.Empty<byte>(); 
            int len = hex.Length; 
            var res = new byte[len / 2]; 
            for (int i = 0; i < len; i += 2) res[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16); 
            return res; 
        } 

        private static void ClearBytes(byte[] b) 
        { 
            if (b == null) return; 
            Array.Clear(b, 0, b.Length);
        } 

        public void Dispose() 
        {
            rng?.Dispose(); 
        } 
    } 
} 
