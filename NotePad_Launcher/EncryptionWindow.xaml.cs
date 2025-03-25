using System.Windows;
using System.Windows.Input;

namespace NotePad_Launcher
{
    /// <summary>
    /// Логика взаимодействия для EncryptionWindow.xaml
    /// </summary>
    public partial class EncryptionWindow : Window
    {
        public EncryptionWindow()
        {
            InitializeComponent();
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
