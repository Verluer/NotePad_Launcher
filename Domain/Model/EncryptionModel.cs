namespace Domain.Model;

public class EncryptionModel
{
    public string FileText { get; set; }
    public string PrimeP { get; set; }
    public string PrimeQ { get; set; }
    public string PrimeE { get; set; }
    public string ModulusN { get; set; }
    public string CloseKeyD { get; set; }

}