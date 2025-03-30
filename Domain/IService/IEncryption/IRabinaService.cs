using Domain.Model;

namespace Domain.IService.IEncryption;

public interface IRabinaService
{
    public EncryptionModel Encryption(EncryptionModel model);
    public EncryptionModel Decryption(EncryptionModel model);
}