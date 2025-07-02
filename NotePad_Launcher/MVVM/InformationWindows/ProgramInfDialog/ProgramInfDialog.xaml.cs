using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;
using NotePad_Launcher.ViewModels.MainWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotePad_Launcher.MVVM.ProgramInfDialog
{
    /// <summary>
    /// Логика взаимодействия для ProgramInfDialog.xaml
    /// </summary>
    public partial class ProgramInfDialog : Window
    {
        public ProgramInfDialog()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<ProgramInfDialogVM>();
            this.DataContext = viewModel;
            viewModel.CloseRequested += OnCloseRequested;
        }
        private void OnCloseRequested()
        {
            this.Close();
        }
        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
