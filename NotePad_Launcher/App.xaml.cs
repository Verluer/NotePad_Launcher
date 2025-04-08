using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService;
using Domain.IService.IEncryption;
using NotePad_Launcher.MVVM.FontPickerDialog;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using NotePad_Launcher.MVVM.SettingsDialog;
using Service;
using Service.Encryption;
using NotePad_Launcher.ViewModels.MainWindow;
using NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.FontPickerDialog;
using NotePad_Launcher.MVVM.FunctionalWindows.FileListWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.EncryptionWindow;
using NotePad_Launcher.MVVM.FunctionalWindows.SearchWindow;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
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
            // Регистрация UI-логики
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddSingleton<IFileDialog, FileDialog>();
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddSingleton<IFileAssociationService, FileAssociationService>();
            services.AddSingleton<IWindowService, WindowService>();
            // Регистрация ViewModels
            services.AddSingleton<MainWindowVM>();
            services.AddTransient<FileListWindowVM>();
            services.AddTransient<EncryptionWindowVM>();
            services.AddTransient<ProgramInfDialogVM>();
            services.AddTransient<FontPickerDialogVM>();
            services.AddTransient<SettingsDialogVM>();
            services.AddTransient<SearchWindowVM>();
            // Регистрация окон
            services.AddSingleton<MainWindow>();
            services.AddTransient<EncryptionWindow>();
            services.AddTransient<FileListWindow>();
            services.AddTransient<ProgramInfDialog>();
            services.AddTransient<FontPickerDialog>();
            services.AddTransient<SettingsDialog>();
            services.AddTransient<SearchWindow>();

        }
    }

}