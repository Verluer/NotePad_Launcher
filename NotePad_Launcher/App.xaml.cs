using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService;
using Domain.IService.IEncryption;
using NotePad_Launcher.Contracts;
using NotePad_Launcher.Services;
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
            // Регистрация зависимостей
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IRSAService, RSAService>();
            services.AddSingleton<IElgamalService, ElgamalService>();
            services.AddSingleton<IRabinaService, RabinaService>();
            services.AddSingleton<IECCService, ECCService>();
            //
            services.AddSingleton<IStringService, StringService>();
            services.AddSingleton<IFileDialog, FileDialog>();
            // Регистрация ViewModels как Transient, если нужно создавать новый экземпляр для каждого окна
            services.AddTransient<MainWindowVM>();
            services.AddTransient<FileListWindowVM>();
            services.AddTransient<EncryptionWindowVM>();
            // Регистрация главного окна
            services.AddSingleton<MainWindow>();
            services.AddSingleton<EncryptionWindow>();
            services.AddSingleton<FileListWindow>();

        }
    }

}