using Domain.IService.IEncryption;
using Domain.Model;
using System.Numerics;
using System.Globalization;
using System.Text;
using Domain.Attributes;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;

namespace Service.Encryption;

[RegisterService(ServiceLifetime.Singleton, serviceType: typeof(IRSAService))]
public class RSAService : IRSAService
{
    #region Helpers

    private (BigInteger g, BigInteger x, BigInteger y) ExtendedGcd(BigInteger a, BigInteger b)
    {
        if (b == 0) return (BigInteger.Abs(a), a.Sign >= 0 ? BigInteger.One : BigInteger.MinusOne, BigInteger.Zero);
        BigInteger x0 = BigInteger.One, x1 = BigInteger.Zero;
        BigInteger y0 = BigInteger.Zero, y1 = BigInteger.One;
        BigInteger aa = a, bb = b;
        while (bb != 0)
        {
            BigInteger q = aa / bb;
            BigInteger tmp = aa % bb; aa = bb; bb = tmp;
            tmp = x0 - q * x1; x0 = x1; x1 = tmp;
            tmp = y0 - q * y1; y0 = y1; y1 = tmp;
        }
        return (BigInteger.Abs(aa), x0, y0);
    }
    private BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        var (g, x, y) = ExtendedGcd(a, m);
        if (g != 1) throw new ArgumentException("Inverse does not exist (gcd != 1).");
        BigInteger res = (x % m + m) % m;
        return res;
    }

    private BigInteger Gcd(BigInteger a, BigInteger b)
    {
        a = BigInteger.Abs(a); b = BigInteger.Abs(b);
        while (b != 0)
        {
            BigInteger t = a % b;
            a = b;
            b = t;
        }
        return a;
    }

    private BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus) =>
        BigInteger.ModPow(value, exponent, modulus);

    // Hash message (SHA-256) -> positive BigInteger
    private BigInteger HashToBigInteger(string message)
    {
        if (message == null) message = string.Empty;
        using (var sha = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            byte[] hash = sha.ComputeHash(bytes);
            // BigInteger expects little-endian; append 0 to ensure positive
            byte[] le = new byte[hash.Length + 1];
            Array.Copy(hash, 0, le, 0, hash.Length);
            le[le.Length - 1] = 0;
            return new BigInteger(le); // little-endian positive
        }
    }

    #endregion
    public EncryptionModel Encryption(EncryptionModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrEmpty(model.FileText)) return new EncryptionModel { FileText = "" };

        BigInteger modulus_n;
        BigInteger value_e;
        BigInteger d = BigInteger.Zero;

        if (!string.IsNullOrEmpty(model.ModulusN) && !string.IsNullOrEmpty(model.PrimeE))
        {
            modulus_n = BigInteger.Parse(model.ModulusN, CultureInfo.InvariantCulture);
            value_e = BigInteger.Parse(model.PrimeE, CultureInfo.InvariantCulture);
        }
        else
        {
            var value_p = BigInteger.Parse(model.PrimeP, CultureInfo.InvariantCulture);
            var value_q = BigInteger.Parse(model.PrimeQ, CultureInfo.InvariantCulture);
            value_e = BigInteger.Parse(model.PrimeE, CultureInfo.InvariantCulture);

            modulus_n = value_p * value_q;
            BigInteger phi = (value_p - 1) * (value_q - 1);
            if (Gcd(value_e, phi) != 1) throw new ArgumentException("e and phi(n) are not coprime.");
            d = ModInverse(value_e, phi);
        }

        byte[] plainBytes = Encoding.UTF8.GetBytes(model.FileText);
        string[] encryptedParts = new string[plainBytes.Length];
        for (int i = 0; i < plainBytes.Length; i++)
        {
            BigInteger m = new BigInteger(plainBytes[i]);
            BigInteger c = ModPow(m, value_e, modulus_n);
            encryptedParts[i] = c.ToString();
        }

        var resultEncryption = string.Join("&", encryptedParts);

        return new EncryptionModel
        {
            CloseKeyD = d != 0 ? d.ToString() : "",
            PrimeE = value_e.ToString(),
            ModulusN = modulus_n.ToString(),
            FileText = resultEncryption
        };
    }

    public EncryptionModel Decryption(EncryptionModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrEmpty(model.FileText)) return new EncryptionModel { FileText = "" };

        if (string.IsNullOrEmpty(model.CloseKeyD) || string.IsNullOrEmpty(model.ModulusN))
            throw new ArgumentException("Для дешифрования требуются CloseKeyD и ModulusN.");

        BigInteger d = BigInteger.Parse(model.CloseKeyD, CultureInfo.InvariantCulture);
        BigInteger n = BigInteger.Parse(model.ModulusN, CultureInfo.InvariantCulture);

        var parts = model.FileText.Split('&', StringSplitOptions.RemoveEmptyEntries);
        byte[] outputBytes = new byte[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            string token = parts[i].Trim();
            if (!BigInteger.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out BigInteger c))
            {
                outputBytes[i] = (byte)'?';
                continue;
            }
            BigInteger m = ModPow(c, d, n);
            if (m < 0 || m > 255) outputBytes[i] = (byte)'?';
            else outputBytes[i] = (byte)m;
        }

        string resultDecryption = Encoding.UTF8.GetString(outputBytes);
        return new EncryptionModel
        {
            FileText = resultDecryption
        };
    }

    public EncryptionModel Signature(EncryptionModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrEmpty(model.FileText)) return new EncryptionModel { FileText = "" };

        if (!string.IsNullOrEmpty(model.ModulusN) && !string.IsNullOrEmpty(model.PrimeE))
        {
            if (!BigInteger.TryParse(model.ModulusN, NumberStyles.Integer, CultureInfo.InvariantCulture, out BigInteger modulus_n) ||
                !BigInteger.TryParse(model.PrimeE, NumberStyles.Integer, CultureInfo.InvariantCulture, out BigInteger value_e))
            {
                return new EncryptionModel { Signature = false, FileText = "" };
            }

            int sepIndex = model.FileText.IndexOf('#');
            if (sepIndex == -1)
            {
                return new EncryptionModel { Signature = false, FileText = "" };
            }

            string message = model.FileText.Substring(0, sepIndex);
            string sigText = model.FileText.Substring(sepIndex + 1);

            string prefix = "Цифровий підпис:";
            int posPrefix = sigText.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
            if (posPrefix >= 0)
                sigText = sigText.Substring(posPrefix + prefix.Length);

            sigText = sigText.Trim();
            if (!BigInteger.TryParse(sigText, NumberStyles.Integer, CultureInfo.InvariantCulture, out BigInteger signatureValue))
            {
                return new EncryptionModel { Signature = false, FileText = "" };
            }

            BigInteger recovered = ModPow(signatureValue, value_e, modulus_n);

            BigInteger actualHash = HashToBigInteger(message);
            BigInteger actualHashMod = actualHash % modulus_n;

            bool ok = recovered == actualHashMod;
            return new EncryptionModel { Signature = ok, FileText = "" };
        }
        else
        {
            BigInteger d;
            BigInteger n;
            try
            {
                if (!string.IsNullOrEmpty(model.CloseKeyD) && !string.IsNullOrEmpty(model.ModulusN))
                {
                    d = BigInteger.Parse(model.CloseKeyD, CultureInfo.InvariantCulture);
                    n = BigInteger.Parse(model.ModulusN, CultureInfo.InvariantCulture);
                }
                else
                {
                    var p = BigInteger.Parse(model.PrimeP, CultureInfo.InvariantCulture);
                    var q = BigInteger.Parse(model.PrimeQ, CultureInfo.InvariantCulture);
                    var e = BigInteger.Parse(model.PrimeE, CultureInfo.InvariantCulture);
                    n = p * q;
                    BigInteger phi = (p - 1) * (q - 1);
                    if (Gcd(e, phi) != 1) throw new ArgumentException("e and phi(n) not coprime.");
                    d = ModInverse(e, phi);
                }
            }
            catch
            {
                return new EncryptionModel { FileText = "", Signature = false };
            }

            string message = model.FileText ?? string.Empty;
            BigInteger h = HashToBigInteger(message);
            BigInteger hMod = h % n;
            BigInteger S = ModPow(hMod, d, n);

            string signatureText = $"{message}#Цифровий підпис: {S.ToString()}";
            return new EncryptionModel
            {
                FileText = signatureText,
                CloseKeyD = d.ToString(),
                ModulusN = n.ToString()
            };
        }
    }
    public RSA CreateRsaKeyPair(int keySize, string saveDirectory, string mode, string nameBase)
    {
        var rsa = RSA.Create(keySize);

        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);

        string publicKeyPath = Path.Combine(saveDirectory, $"{nameBase}_{mode}_public_key.pem");
        string privateKeyPath = Path.Combine(saveDirectory, $"{nameBase}_{mode}_private_key.pem");

        string publicPem = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
        publicPem = "-----BEGIN PUBLIC KEY-----\n" +
                    string.Join('\n', SplitBase64(publicPem)) +
                    "\n-----END PUBLIC KEY-----";

        string privatePem = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());
        privatePem = "-----BEGIN PRIVATE KEY-----\n" +
                     string.Join('\n', SplitBase64(privatePem)) +
                     "\n-----END PRIVATE KEY-----";

        File.WriteAllText(publicKeyPath, publicPem);
        File.WriteAllText(privateKeyPath, privatePem);

        return rsa;
    }
    public X509Certificate2 CreateSelfSignedCertificate(RSA rsa, string subjectName)
    {
        var req = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var notBefore = DateTimeOffset.UtcNow.AddDays(-1);
        var notAfter = notBefore.AddYears(5);
        var cert = req.CreateSelfSigned(notBefore, notAfter);
        return cert.CopyWithPrivateKey(rsa);
    }
    private static IEnumerable<string> SplitBase64(string base64)
    {
        for (int i = 0; i < base64.Length; i += 64)
            yield return base64.Substring(i, Math.Min(64, base64.Length - i));
    }
}
