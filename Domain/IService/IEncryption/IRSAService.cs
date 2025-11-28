using Domain.Model;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Domain.IService.IEncryption;

public interface IRSAService
{
    public EncryptionModel Encryption(EncryptionModel model);
    public EncryptionModel Decryption(EncryptionModel model);
    public EncryptionModel Signature(EncryptionModel model);
    public RSA CreateRsaKeyPair(int keySize, string saveDirectory, string mode, string nameBase);
    public X509Certificate2 CreateSelfSignedCertificate(string subjectName, string savePath, string password, int keySize = 2048, int validYears = 5);
}