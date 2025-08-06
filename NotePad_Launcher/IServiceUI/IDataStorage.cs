
using System.Windows;
using Domain.Enum;
using Domain.Model;
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

    public Func<(int index, int length)> GetSelectionCallback { get; set; }
    public Func<int> GetCaretOffset { get; set; }
    EncryptionMethod CurrentMethod { get; set; }
    SearchReplaceMethod searchReplaceMethod { get; set; }
    public string StartupFilePath { get; set; }

    public event Action<FileModel, bool> SelectionFileUpdated;
    public void PushUpdatedSelectionFile(FileModel selectionFile, bool deleteFile);
}