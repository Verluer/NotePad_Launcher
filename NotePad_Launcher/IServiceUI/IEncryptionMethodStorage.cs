using Domain.Enum;

namespace NotePad_Launcher.Contracts;

public interface IEncryptionMethodStorage
{
    EncryptionMethod CurrentMethod { get; set; }
}