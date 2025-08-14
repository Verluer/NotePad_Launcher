using Domain.IService.IFileSystem;
using Domain.IService.ISystemApp;
using Domain.IService.ITextUtils;
using Domain.IService.IValidation;
using Domain.Model;
using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.MVVM.DialogWindows.InputTextDialog;
using Service.SystemApp;
using SharpVectors.Converters;
using System.Reflection;
using System.Windows;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; set; }
        public static AppConfigModel Config { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);

            var services = new ServiceCollection();

            System.Threading.Tasks.Task.Run(() => PreloadSvgIcons());

            services.AddServicesWithAttributes(
            Assembly.GetExecutingAssembly(),
            typeof(ServiceCollectionExtensions).Assembly);

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            if (e.Args.Length > 0)
            {
                LoadServices(ServiceProvider, e.Args.Length, e.Args[0]);
            }
            else
            {
                LoadServices(ServiceProvider, 0, null);
            }
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        private static void LoadServices(IServiceProvider serviceProvider, int eArgsLength, string eArgs)
        {
            var configService = serviceProvider.GetRequiredService<IConfigService>();
            Config = configService.Load();

            var localization = serviceProvider.GetRequiredService<ILocalizationService>();
            localization.LoadLanguage(App.Config.Language);

            var dataStorage = serviceProvider.GetRequiredService<IDataStorage>();
            if (eArgsLength > 0)
            {
                string filePath = eArgs;
                dataStorage.StartupFilePath = filePath;
            }
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var windowService = serviceProvider.GetRequiredService<IWindowService>();
            var fileDialog = serviceProvider.GetRequiredService<IFileDialog>();
            var validationService = serviceProvider.GetRequiredService<IValidationService>();
            var directoryService = serviceProvider.GetRequiredService<IDirectoryService>();
            var textService = serviceProvider.GetRequiredService<ITextService>();
            var fileSystemCore = serviceProvider.GetRequiredService<IFileCoreService>();
            var fileSystemManager = serviceProvider.GetRequiredService<IFileSystemManager>();

            var fileAssociationService = serviceProvider.GetRequiredService<IFileAssociationService>();
            fileAssociationService.RegisterTxtFileAssociation();


        }
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<InputTextDialogVM>(sp =>
            { 
                throw new InvalidOperationException("Use factory method to create InputTextDialogVM with parameters.");
            });
            services.AddTransient<Func<string, string, string, bool, bool, InputTextDialogVM>>(sp => (title, message, input, isTextBox, isComboBox) =>
            {
                return new InputTextDialogVM(title, message, input, isTextBox, isComboBox);
            });

        }
        private void PreloadSvgIcons()
        {
            var thread = new Thread(() =>
            {
                var iconUris = new[]
            {
            new Uri("pack://application:,,,/Resources/Svg/Explorer.svg"),
            new Uri("pack://application:,,,/Resources/Svg/FileUpdate.svg"),
            new Uri("pack://application:,,,/Resources/Svg/FolderAdd.svg"),
            new Uri("pack://application:,,,/Resources/Svg/FolderDelete.svg"),
            new Uri("pack://application:,,,/Resources/Svg/FolderEdit.svg"),
            new Uri("pack://application:,,,/Resources/Svg/Refresh.svg"),
            new Uri("pack://application:,,,/Resources/Svg/TxtFileAdd.svg"),

        };

                foreach (var uri in iconUris)
                {
                    var svgViewbox = new SvgViewbox
                    {
                        Source = uri
                    };

                    svgViewbox.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    svgViewbox.Arrange(new Rect(0, 0, 1, 1));
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
    }
}