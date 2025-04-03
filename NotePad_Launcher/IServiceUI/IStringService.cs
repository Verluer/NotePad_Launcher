using System.Windows.Media;

namespace NotePad_Launcher;

public interface IStringService
{
    Func<string> GetTextCallback { get; set; }
    event Action<string> TextUpdated;

    void PushUpdatedText(string updatedText);

    /// <summary>
    /// ///////////////////////////////////////
    /// </summary>
    Func<(double fontSize, FontFamily fontFamily)> GetFontFamilySizeCallback { get; set; }

    event Action<double, FontFamily> FamilySizeUpdated;

    void PushUpdatedFamilySize(double updatedFontSize, FontFamily updatedFontFamily);

}