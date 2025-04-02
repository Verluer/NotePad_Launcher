using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService;
using Domain.IService.IEncryption;
using NotePad_Launcher.MVVM.ProgramInfDialog;
using NotePad_Launcher.ViewModels.EncryptionWindow;
using NotePad_Launcher.ViewModels.FileListWindow;
using Service;
using Service.Encryption;
using NotePad_Launcher.ViewModels.MainWindow;

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
            // Регистрация UI-логики
            services.AddSingleton<IStringService, StringService>();
            services.AddSingleton<IFileDialog, FileDialog>();
            services.AddSingleton<IEncryptionMethodStorage, EncryptionMethodStorage>();
            services.AddSingleton<IStringService, StringService>();
            // Регистрация ViewModels
            services.AddSingleton<MainWindowVM>();
            services.AddTransient<FileListWindowVM>();
            services.AddTransient<EncryptionWindowVM>();
            services.AddTransient<ProgramInfDialogVM>();
            // Регистрация окон
            services.AddSingleton<MainWindow>();
            services.AddTransient<EncryptionWindow>();
            services.AddTransient<FileListWindow>();
            services.AddTransient<ProgramInfDialog>();

        }
    }

}