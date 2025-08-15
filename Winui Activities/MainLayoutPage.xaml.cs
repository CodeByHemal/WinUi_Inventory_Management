using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using WinUi_Inventory_Management.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi_Inventory_Management.Winui_Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainLayoutPage : Page
    {
        private User _loggedInUser;
        public MainLayoutPage()
        {
            InitializeComponent();
            

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is User user)
            {
                _loggedInUser = user;

                // Navigate to DashboardPage with user info
                ContentFrame.Navigate(typeof(DashboardPage), _loggedInUser);
                RootNavView.SelectedItem = RootNavView.MenuItems[0];
            }
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
                    "SettingsPage" => typeof(SettingsPage),
                    _ => typeof(DashboardPage)
                };


                if (pageType != null)
                    ContentFrame.Navigate(pageType,_loggedInUser);
            }
        }

    }
}
