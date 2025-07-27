using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using Microsoft.UI.Windowing;
using WinRT.Interop;
using System.Collections.Generic;
using WinUi_Inventory_Management.Winui_Activities;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        public MainWindow()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(SplashPage));
        }
    }
}
