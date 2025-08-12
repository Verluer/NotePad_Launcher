
using System.Windows;
using System.Windows.Media;
using Domain.Enum;
using Domain.Model;

namespace NotePad_Launcher;

public class DataStorage : IDataStorage
{
    #region хранение данных
    public Func<string> GetTextCallback { get; set; }
    public Func<(int index, int length)> GetSelectionCallback { get; set; }
    public Func<int> GetCaretOffset { get; set; }
    public EncryptionMethod CurrentMethod { get; set; }
    public SearchReplaceMethod searchReplaceMethod { get; set; }
    public string StartupFilePath { get; set; }
    public List<string> AllHighlightings { get; set; } = new List<string>();

    public event Action? UpdateSyntaxHighlighting;
    public void PushUpdatedSyntax()
    {
        UpdateSyntaxHighlighting?.Invoke();
    }
    #endregion

    #region Передача измененной строки
    public event Action<string> TextUpdated;


    public void PushUpdatedText(string updatedText)
    {
        TextUpdated?.Invoke(updatedText);
    }
    #endregion

    #region Передача найденного паттерна в тексте

    public event Action<int, int> SearchAction;

    public void ResultSearch(int IndexSearch, int LengthSearch)
    {
        SearchAction?.Invoke(IndexSearch, LengthSearch);
    }
    #endregion

    #region Передача-хранение стилей основного текста
    public Func<(double fontSize, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight)> GetFontFamilySizeCallback { get; set; }

    public event Action<double, FontFamily, FontStyle, FontWeight> FamilySizeUpdated;
    public void PushUpdatedFamilySize(double updatedFontSize, FontFamily updatedFontFamily, FontStyle updatedFontStyle, FontWeight updatedFontWeight)
    {
        FamilySizeUpdated?.Invoke(updatedFontSize, updatedFontFamily, updatedFontStyle, updatedFontWeight);
    }
    #endregion

    #region FileList Selection

    public event Action<FileModel, bool> SelectionFileUpdated;
    public void PushUpdatedSelectionFile(FileModel selectionFile, bool deleteFile)
    {
        SelectionFileUpdated?.Invoke(selectionFile, deleteFile);
    }
    #endregion

}