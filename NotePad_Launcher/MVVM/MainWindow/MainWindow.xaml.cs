using System.Windows;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDataStorage _dataStorage;
        public MainWindow(IDataStorage dataStorage)
        {
            InitializeComponent();
            _dataStorage = dataStorage;
            _dataStorage.SearchAction += TextFound;
            var viewModel = App.ServiceProvider.GetRequiredService<MainWindowVM>();
            this.DataContext = viewModel;
            FileText.Document = viewModel.FileTextDocument;
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.UpdateWordWrapAction = () =>
            {
                FileText.WordWrap = viewModel.IsWordWrapEnabled;
            };
        }
        private void OnMinimizeRequested()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRequested()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void TextFound(int IndexSearch, int LengthSearch)
        {
            FileText.Select(IndexSearch, LengthSearch);
            var location = FileText.Document.GetLocation(IndexSearch);
            FileText.ScrollTo(location.Line, location.Column);
        }
    }
}