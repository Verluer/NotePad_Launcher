using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService.IEncryption
{
    public interface IG28147Service
    {
        public EncryptionModel Encryption(EncryptionModel model);
        public EncryptionModel Decryption(EncryptionModel model);
    }
}
