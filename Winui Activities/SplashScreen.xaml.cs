using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SplashScreen : Window
{
    public SplashScreen()
    {
        InitializeComponent();
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, object e)
    {
        // Stop the timer
        (sender as DispatcherTimer)?.Stop();

        // Create and show the new window (replace MainWindow with your target window)
        var mainWindow = new WinUi_Inventory_Management.MainWindow();
        mainWindow.Activate();

        // Close the splash screen
        this.Close();
    }
}
