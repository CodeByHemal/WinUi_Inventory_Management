using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUi_Inventory_Management.Models;
using WinUi_Inventory_Management.ViewModels;

namespace WinUi_Inventory_Management.Winui_Activities
{
    public sealed partial class OrdersPage : Page
    {
        public OrdersViewModel ViewModel { get; } = new OrdersViewModel();

        public OrdersPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        // Generic helper to execute commands
        private void ExecuteProductCommand(object sender, IRelayCommand<Product> command)
        {
            if (sender is Button btn && btn.DataContext is Product product)
            {
                command.Execute(product);
            }
        }

        private void Increment_Quantity_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.IncrementQuantityCommand);

        private void Decrement_Quantity_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.DecrementQuantityCommand);

        private void Delete_Product_Btn(object sender, RoutedEventArgs e)
            => ExecuteProductCommand(sender, ViewModel.RemoveProductCommand);
    }
}
