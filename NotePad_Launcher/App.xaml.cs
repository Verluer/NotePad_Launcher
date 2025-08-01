using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService;
using Domain.IService.IEncryption;
using NotePad_Launcher.MVVM.FontPickerDialog;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using Service;
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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Domain.Model;
using NotePad_Launcher.MVVM.DialogWindows.InputTextDialog;

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

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();

            var _ = ServiceProvider.GetRequiredService<ILocalizationService>();
            if (e.Args.Length > 0)
            {
                string filePath = e.Args[0];
                var dataStorage = ServiceProvider.GetRequiredService<IDataStorage>();
                dataStorage.StartupFilePath = filePath;
            }
            var configService = ServiceProvider.GetRequiredService<IConfigService>();
            Config = configService.Load();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            LocalizationService.Instance.LoadLanguage(App.Config.Language);
            mainWindow.Show();
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
    }

}