using Domain.Model;

namespace Domain.IService.IEncryption;

public interface IECCService
{
    public EncryptionModel Encryption(EncryptionModel model);
    public EncryptionModel Decryption(EncryptionModel model);
    public EncryptionModel Signature(EncryptionModel model);
}