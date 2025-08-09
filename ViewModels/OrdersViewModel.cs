using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WinUi_Inventory_Management.Models;

namespace WinUi_Inventory_Management.ViewModels
{
    public partial class OrdersViewModel : ObservableObject
    {
        // Input fields
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private double price;

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private double discount;

        // Error message properties
        [ObservableProperty]
        private string nameError;

        [ObservableProperty]
        private string priceError;

        [ObservableProperty]
        private string quantityError;

        [ObservableProperty]
        private string discountError;

        // Observable collection of products
        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        public OrdersViewModel()
        {
            Price = 0;
            Quantity = 0;
            Discount = 0;
        }

        // Properties to indicate if error messages should be visible 
        public bool IsNameErrorVisible => !string.IsNullOrEmpty(NameError);
        public bool IsPriceErrorVisible => !string.IsNullOrEmpty(PriceError);
        public bool IsQuantityErrorVisible => !string.IsNullOrEmpty(QuantityError);
        public bool IsDiscountErrorVisible => !string.IsNullOrEmpty(DiscountError);

        // Notify changes on error message properties to update visibility bindings
        partial void OnNameErrorChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(IsNameErrorVisible));
        }

        partial void OnPriceErrorChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(IsPriceErrorVisible));
        }

        partial void OnQuantityErrorChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(IsQuantityErrorVisible));
        }

        partial void OnDiscountErrorChanged(string oldValue, string newValue)
        {
            OnPropertyChanged(nameof(IsDiscountErrorVisible));
        }

        // Input property change validation
        partial void OnNameChanged(string oldValue, string newValue)
        {
            ValidateName();
            AddProductCommand.NotifyCanExecuteChanged();
        }

        partial void OnPriceChanged(double oldValue, double newValue)
        {
            ValidatePrice();
            AddProductCommand.NotifyCanExecuteChanged();
        }

        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            ValidateQuantity();
            AddProductCommand.NotifyCanExecuteChanged();
        }

        partial void OnDiscountChanged(double oldValue, double newValue)
        {
            ValidateDiscount();
            AddProductCommand.NotifyCanExecuteChanged();
        }

        // Validation methods
        private void ValidateName()
        {
            NameError = string.IsNullOrWhiteSpace(Name) ? "Name cannot be empty." : string.Empty;
        }

        private void ValidatePrice()
        {
            PriceError = Price <= 0 ? "Price must be greater than 0." : string.Empty;
        }

        private void ValidateQuantity()
        {
            QuantityError = Quantity <= 0 ? "Quantity must be greater than 0." : string.Empty;
        }

        private void ValidateDiscount()
        {
            if (Discount < 0)
                DiscountError = "Discount cannot be negative.";
            else if (Discount > Price)
                DiscountError = "Discount cannot exceed price.";
            else
                DiscountError = string.Empty;
        }

        // AddProduct command with validation
        [RelayCommand(CanExecute = nameof(CanAddProduct))]
        private void AddProduct()
        {
            var product = new Product
            {
                Name = Name,
                Quantity = Quantity,
                Price = Price * Quantity,
                Discount = Discount
            };
            Products.Add(product);

            // Clear inputs
            Name = string.Empty;
            Price = 0;
            Quantity = 0;
            Discount = 0;
        }

        [RelayCommand]
        private void IncrementQuantity(Product product)
        {
            product.Quantity += 1;
            Quantity = product.Quantity;
        }

        [RelayCommand]
        private void DecrementQuantity(Product product)
        {
            if (product.Quantity > 1)
            {
                product.Quantity -= 1;
            }
        }
        private bool CanAddProduct()
        {
            ValidateName();
            ValidatePrice();
            ValidateQuantity();
            ValidateDiscount();

            return string.IsNullOrEmpty(NameError)
                && string.IsNullOrEmpty(PriceError)
                && string.IsNullOrEmpty(QuantityError)
                && string.IsNullOrEmpty(DiscountError);
        }

    }
}
