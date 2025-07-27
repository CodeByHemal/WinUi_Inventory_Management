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

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            _appWindow = AppWindow.GetFromWindowId(windowId);

            // Match with dark theme background (recommended values)
            var titleBar = _appWindow.TitleBar;
            titleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);  // near WinUI dark background
            titleBar.ForegroundColor = Colors.White;

            titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
            titleBar.ButtonForegroundColor = Colors.White;

            titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 64, 64, 64);
            titleBar.ButtonHoverForegroundColor = Colors.White;

            titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(255, 96, 96, 96);
            titleBar.ButtonPressedForegroundColor = Colors.White;

            titleBar.InactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
            titleBar.InactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 32, 32, 32);
            titleBar.ButtonInactiveForegroundColor = Colors.Gray;

            MainFrame.Navigate(typeof(SplashPage));
        }
    }
}
