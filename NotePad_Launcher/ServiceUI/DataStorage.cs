
using System.Windows;
using System.Windows.Media;
using Domain.Enum;

namespace NotePad_Launcher;

public class DataStorage : IDataStorage
{
    #region хранение string
    public Func<string> GetTextCallback { get; set; }
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

    #region Хранение выбранного метода шифрования
    public EncryptionMethod CurrentMethod { get; set; }
    #endregion

    #region Изначальный путь файла
    public string StartupFilePath { get; set; }
    #endregion
}