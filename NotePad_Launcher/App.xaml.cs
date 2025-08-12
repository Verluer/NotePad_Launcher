using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService.IEncryption;
using NotePad_Launcher.MVVM.FontPickerDialog;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using Service.Encryption;
using NotePad_Launcher.ViewModels.MainWindow;
using NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.FontPickerDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.FileListWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.EncryptionWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.SettingsWindow;
using NotePad_Launcher.IServiceUI;
using NotePad_Launcher.ServiceUI;
using SharpVectors.Converters;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Domain.Model;
using NotePad_Launcher.MVVM.DialogWindows.InputTextDialog;
using System;
using Service.FileSystem;
using Service.TextUtils;
using Service.SystemApp;
using Domain.IService.ITextUtils;
using Domain.IService.ISystemApp;
using Domain.IService.IFileSystem;
using Service.Validation;
using Domain.IService.IValidation;

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
            // Регистрация сервайс-логики
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IRSAService, RSAService>();
            services.AddSingleton<IElgamalService, ElgamalService>();
            services.AddSingleton<IRabinaService, RabinaService>();
            services.AddSingleton<IECCService, ECCService>();
            services.AddSingleton<ISearchService, SearchService>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IValidationService, ValidationService>();
            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<ITextService, TextService>();
            services.AddSingleton<IFileCoreService, FileCoreService>();
            services.AddSingleton<IFileSystemManager, FileSystemManager>();
            // Регистрация UI-логики
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddSingleton<IFileDialog, FileDialog>();
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddSingleton<IFileAssociationService, FileAssociationService>();
            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            // Регистрация ViewModels
            services.AddSingleton<MainWindowVM>();
            services.AddTransient<FileListWindowVM>();
            services.AddTransient<EncryptionWindowVM>();
            services.AddTransient<ProgramInfDialogVM>();
            services.AddTransient<FontPickerDialogVM>();
            services.AddTransient<SearchWindowVM>();
            services.AddTransient<SettingsWindowVM>();
            // Регистрация окон
            services.AddSingleton<MainWindow>();
            services.AddTransient<EncryptionWindow>();
            services.AddTransient<FileListWindow>();
            services.AddTransient<ProgramInfDialog>();
            services.AddTransient<FontPickerDialog>();
            services.AddTransient<SearchWindow>();
            services.AddTransient<SettingsWindow>();
            // Регистрация диалогов 
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