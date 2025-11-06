using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.IEncryption
{
    public interface IAESService
    {
        public EncryptionModel Encryption(EncryptionModel model, string recipientPublicPemPath, string signerPrivatePemPath);
        public EncryptionModel Decryption(EncryptionModel model, string pathPrivateKey, string pathPublicKey);
        public void SaveEncryptedBundle(string path, string nameBase, EncryptionModel result);
        public EncryptionMetadata LoadEncryptedBundle(string path);
    }
}
