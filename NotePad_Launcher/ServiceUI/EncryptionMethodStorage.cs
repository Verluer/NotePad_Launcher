using Domain.Enum;

namespace NotePad_Launcher;

public class EncryptionMethodStorage : IEncryptionMethodStorage
{
    public EncryptionMethod CurrentMethod { get; set; }
}
