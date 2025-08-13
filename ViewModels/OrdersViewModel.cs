using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        [ObservableProperty]
        private double totalDiscountAmount = 0;

        [ObservableProperty]
        private double allProductTotalPrice = 0;


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
                Price = Price,
                Discount = Discount, 
                TotalPrice = (Price * Quantity) - Discount
            };
            Products.Add(product);

            // Clear inputs
            Name = string.Empty;
            Price = 0;
            Quantity = 0;
            Discount = 0;
            TotalDiscountAmount = GetTotalDiscountAmount();
            AllProductTotalPrice = GetAllProductTotalPrice();
        }

        [RelayCommand]
        private void IncrementQuantity(Product product)
        {
            var index = Products.IndexOf(product);
            Products[index].Quantity += 1;
            Products[index].TotalPrice = (Products[index].Price * Products[index].Quantity) - Products[index].Discount;
            AllProductTotalPrice = GetAllProductTotalPrice();

        }

        [RelayCommand]
        private void DecrementQuantity(Product product)
        {
            var index = Products.IndexOf(product);
            if (Products[index].Quantity > 1)
            {
                Products[index].Quantity -= 1;
                Products[index].TotalPrice = (Products[index].Price * Products[index].Quantity) - Products[index].Discount;
                if (Products[index].Discount > Products[index].TotalPrice)
                {
                    Products[index].Discount = 0;
                }

                AllProductTotalPrice = GetAllProductTotalPrice();
            }
        }

        [RelayCommand]
        private void RemoveProduct(Product product)
        {
            Products.Remove(product);
            AllProductTotalPrice = GetAllProductTotalPrice();
            TotalDiscountAmount = GetTotalDiscountAmount();
        }

        public double GetTotalDiscountAmount()
        {
            TotalDiscountAmount = Products.Sum(p => p.Discount);
            return TotalDiscountAmount;
        }

        public double GetAllProductTotalPrice()
        {
            AllProductTotalPrice = Products.Sum(p => p.TotalPrice);
            return AllProductTotalPrice;
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
