using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Domain.IService;
using Domain.IService.IEncryption;
using Service;
using Service.Encryption;

namespace NotePad_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceProvider ServiceProvider { get; set; }

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

            // Регистрация главного окна
            services.AddSingleton<MainWindow>();
            services.AddSingleton<EncryptionWindow>();
            services.AddSingleton<FileListWindow>();

        }
    }
}