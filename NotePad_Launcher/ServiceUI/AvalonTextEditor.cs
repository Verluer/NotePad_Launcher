using ICSharpCode.AvalonEdit;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace NotePad_Launcher.Services;

public class AvalonTextEditor : TextEditor, INotifyPropertyChanged
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text", typeof(string), typeof(AvalonTextEditor), new PropertyMetadata(default(string), OnTextChanged));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); OnPropertyChanged(); }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = d as AvalonTextEditor;
        if (editor != null)
        {
            editor.OnPropertyChanged(nameof(Text));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}