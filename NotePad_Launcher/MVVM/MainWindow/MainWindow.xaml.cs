using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Domain.Attributes;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using NotePad_Launcher.ViewModels.MainWindow;

namespace NotePad_Launcher
{
    [RegisterService(ServiceLifetime.Singleton, asSelf: true)]
    public partial class MainWindow : Window
    {
        private readonly IDataStorage _dataStorage;
        private readonly MainWindowVM _viewModel;
        public MainWindow(IDataStorage dataStorage)
        {
            InitializeComponent();

            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            _dataStorage = dataStorage;
            _dataStorage.SearchAction += TextFound;

            var allHighlightings = HighlightingManager.Instance.HighlightingDefinitions;
            foreach (var highlighting in allHighlightings)
            {
                _dataStorage.AllHighlightings.Add(highlighting.Name);
            }

            var viewModel = App.ServiceProvider.GetRequiredService<MainWindowVM>();
            this.DataContext = viewModel;

            _viewModel = viewModel;

            FileText.Document = viewModel.FileTextDocument;
            viewModel.MaximizeRequested += OnMaximizeRequested;
            viewModel.MinimizeRequested += OnMinimizeRequested;
            viewModel.CloseRequested += OnCloseRequested;
            viewModel.UpdateWordWrapRequested += () =>
            {
                FileText.WordWrap = viewModel.IsWordWrapEnabled;
            };
            viewModel.UpdateSyntaxHighlightingRequested += () =>
            {
                if(viewModel.IsSyntaxHighlightingEnabled) 
                FileText.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition($"{App.Config.SyntaxHighlighting}");
                else
                {
                    FileText.SyntaxHighlighting = null;
                }
            };

            _dataStorage.GetSelectionCallback = () =>
            {
                var selection = FileText.TextArea.Selection;
                if (selection.IsEmpty || selection.SurroundingSegment == null)
                {
                    return (0, 0);
                }
                int index = selection.SurroundingSegment.Offset;
                int length = selection.Length;
                return (index, length);
            };
            _dataStorage.GetCaretOffset = () =>
            {
                var offset = FileText.CaretOffset;
                return offset;
            };
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _dataStorage.PushUpdatedSyntax();
            _dataStorage.PushUpdatedWordWrap();
        }
        private void OnMinimizeRequested()
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximizeRequested()
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
        private void OnCloseRequested()
        {
            Application.Current.Shutdown();
        }
        private void OnUpdateWordWrapRequested()
        {

        }
        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    // Позиция курсора относительно экрана
                    var mousePos = e.GetPosition(this);
                    var screenPos = this.PointToScreen(mousePos);

                    // Процент от ширины окна, где кликнули
                    double relativeX = mousePos.X / this.ActualWidth;

                    // Переводим окно в нормальное состояние
                    this.WindowState = WindowState.Normal;

                    // Вычисляем новое положение окна, чтобы курсор "оставался" на том же месте заголовка
                    // Смещаем окно по X, чтобы курсор оказался на нужной позиции внутри окна
                    this.Left = screenPos.X - relativeX * this.Width;
                    this.Top = screenPos.Y - mousePos.Y;

                    // Теперь можно начать перетаскивание
                    this.DragMove();
                }
                else
                {
                    // Если не максимизировано, просто перетаскиваем
                    this.DragMove();
                }
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