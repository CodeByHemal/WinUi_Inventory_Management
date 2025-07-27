using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainLayoutPage : Page
    {
        public MainLayoutPage()
        {
            InitializeComponent();
            ContentFrame.Navigate(typeof(DashboardPage));
            RootNavView.SelectedItem = RootNavView.MenuItems[0]; // Highlight Dashboard in sidebar

        }

        private void RootNavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = sender.SelectedItem as NavigationViewItem;
            if (item != null)
            {
                string pageTag = item.Tag?.ToString();

                Type pageType = pageTag switch
                {
                    "DashboardPage" => typeof(DashboardPage),
                    "InvoicePage" => typeof(InvoicePage),
                    "OrdersPage" => typeof(OrdersPage),
                    _ => typeof(SettingsPage)
                };

                if (pageType != null)
                    ContentFrame.Navigate(pageType);
            }
        }

    }
}
