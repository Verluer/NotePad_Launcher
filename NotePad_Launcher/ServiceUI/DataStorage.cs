
using System.Windows;
using System.Windows.Media;
using Domain.Enum;

namespace NotePad_Launcher;

public class DataStorage : IDataStorage
{
    public Func<string> GetTextCallback { get; set; }

    public event Action<string> TextUpdated;

    public void PushUpdatedText(string updatedText)
    {
        TextUpdated?.Invoke(updatedText);
    }
    public Func<(double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight)> GetFontFamilySizeCallback { get; set; }

    public event Action<double, FontFamily, FontStyle, FontWeight> FamilySizeUpdated;
    public void PushUpdatedFamilySize(double updatedFontSize, FontFamily updatedFontFamily, FontStyle updatedFontStyle, FontWeight updatedFontWeight)
    {
        FamilySizeUpdated?.Invoke(updatedFontSize, updatedFontFamily, updatedFontStyle, updatedFontWeight);
    }

    public EncryptionMethod CurrentMethod { get; set; }

}