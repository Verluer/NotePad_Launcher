
using System.Windows;
using Domain.Enum;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

namespace NotePad_Launcher;

public interface IDataStorage
{
    Func<string> GetTextCallback { get; set; }
    event Action<string> TextUpdated;

    void PushUpdatedText(string updatedText);

    event Action<int, int> SearchAction;
    void ResultSearch(int IndexSearch, int LengthSearch);
    Func<(double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight)> GetFontFamilySizeCallback { get; set; }

    event Action<double, FontFamily, FontStyle, FontWeight> FamilySizeUpdated;

    void PushUpdatedFamilySize(double updatedFontSize, FontFamily updatedFontFamily, FontStyle updatedFontStyle, FontWeight updatedFontWeight);


    EncryptionMethod CurrentMethod { get; set; }
}