using Microsoft.Extensions.DependencyInjection;
using NotePad_Launcher.MVVM.InformationWindows.ProgramInfDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace NotePad_Launcher.MVVM.DialogWindows.InputTextDialog
{
    /// <summary>
    /// Логика взаимодействия для InputTextDialog.xaml
    /// </summary>
    public partial class InputTextDialog : Window
    {
        public InputTextDialog()
        {
            InitializeComponent();
           
            var chrome = new WindowChrome
            {
                CaptionHeight = 0,
                ResizeBorderThickness = new Thickness(6),
                UseAeroCaptionButtons = false
            };
            WindowChrome.SetWindowChrome(this, chrome);

            this.DataContextChanged += InputTextDialog_DataContextChanged;
        }

        private void InputTextDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is InputTextDialogVM oldVm)
            {
                oldVm.CloseRequested -= OnCloseRequested;
                oldVm.OkRequested -= OnOkRequested;
            }

            if (e.NewValue is InputTextDialogVM newVm)
            {
                newVm.CloseRequested += OnCloseRequested;
                newVm.OkRequested += OnOkRequested;
            }
        }
        private void OnCloseRequested()
        {
            this.Close();
        }
        private void OnOkRequested()
        {
            this.DialogResult = true;
        }
        private void HeadLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized)
                {

                    var mousePos = e.GetPosition(this);
                    var screenPos = this.PointToScreen(mousePos);

                    double relativeX = mousePos.X / this.ActualWidth;

                    this.WindowState = WindowState.Normal;

                    this.Left = screenPos.X - relativeX * this.Width;
                    this.Top = screenPos.Y - mousePos.Y;

                    this.DragMove();
                }
                else
                {
                    this.DragMove();
                }
            }
        }
    }
}
