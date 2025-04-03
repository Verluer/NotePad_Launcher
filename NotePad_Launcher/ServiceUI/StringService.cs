
using System.Windows.Media;

namespace NotePad_Launcher;

public class StringService : IStringService
{
    public Func<string> GetTextCallback { get; set; }

    public event Action<string> TextUpdated;

    public void PushUpdatedText(string updatedText)
    {
        TextUpdated?.Invoke(updatedText);
    }
    public Func<(double fontSize, FontFamily fontFamily)> GetFontFamilySizeCallback { get; set; }

    public event Action<double, FontFamily> FamilySizeUpdated;
    public void PushUpdatedFamilySize(double updatedFontSize, FontFamily updatedFontFamily)
    {
        FamilySizeUpdated?.Invoke(updatedFontSize, updatedFontFamily);
    }

}