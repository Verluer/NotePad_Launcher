using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.ProgramInfDialog;
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

namespace NotePad_Launcher.MVVM.FontPickerDialog
{
    /// <summary>
    /// Логика взаимодействия для FontPickerDialog.xaml
    /// </summary>
    public partial class FontPickerDialog : Window
    {
        public FontPickerDialog()
        {
            InitializeComponent();
            var viewModel = App.ServiceProvider.GetRequiredService<FontPickerDialogVM>();
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
