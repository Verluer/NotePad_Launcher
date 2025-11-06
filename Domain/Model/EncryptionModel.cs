using System.Reflection.Metadata;

namespace Domain.Model;

public class EncryptionModel
{
    public string FileText { get; set; }
    public string PrimeP { get; set; }
    public string PrimeQ { get; set; }
    public string PrimeE { get; set; }
    public string ModulusN { get; set; }
    public string CloseKeyD { get; set; }
    public bool Signature { get; set; }
    public byte[] bytes { get; set; }
    public EncryptionMetadata Metadata { get; set; }
}
public class EncryptionMetadata 
{
    public string Algorithm { get; set; } 
    public int KeySize { get; set; } // розмір ключа в бітах (128/192/256)
    public string Mode { get; set; } 
    public string IVHex { get; set; }
    public string SaltHex { get; set; } // соль у HEX (якщо PBKDF2)
    public int Iterations { get; set; } // кількість ітерацій PBKDF2
    public string SignatureHex { get; set; }
    public string WrappedKeyHex { get; set; } 
    public string KeyWrapAlgorithm { get; set; } 
    public string SignatureAlgorithm { get; set; } 
    public long PlaintextLength { get; set; } // довжина оригінального бінарного вмісту
} 