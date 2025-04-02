using Domain.Enum;

namespace NotePad_Launcher;

public interface IEncryptionMethodStorage
{
    EncryptionMethod CurrentMethod { get; set; }
}