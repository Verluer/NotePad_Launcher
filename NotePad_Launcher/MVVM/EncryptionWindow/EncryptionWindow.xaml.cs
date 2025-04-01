using System.Windows;
using System.Windows.Input;
using Domain.Enum;
using Domain.IService.IEncryption;
using Domain.Model;
using NotePad_Launcher.ViewModels.EncryptionWindow;
using NotePad_Launcher.ViewModels.MainWindow;
using Service.Encryption;
using static ICSharpCode.AvalonEdit.Document.TextDocumentWeakEventManager;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        public EncryptionWindow(EncryptionMethod method)
        {
            InitializeComponent();
            var viewModel = new EncryptionWindowVM(method);
            this.DataContext = viewModel;
        }

        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
 
    }
}
