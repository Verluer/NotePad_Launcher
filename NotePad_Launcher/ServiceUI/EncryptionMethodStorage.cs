using Domain.Enum;
using NotePad_Launcher.Contracts;

namespace NotePad_Launcher.Services;

public class EncryptionMethodStorage : IEncryptionMethodStorage
{
    public EncryptionMethod CurrentMethod { get; set; }
}
