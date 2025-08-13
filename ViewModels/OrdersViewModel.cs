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
            if (string.IsNullOrWhiteSpace(Name))
            {
                NameError = "Name cannot be empty.";
            }
            else if (Name.Length > 50)
            {
                NameError = "Name cannot exceed 50 characters.";
            }
            else
            {
                var regex = NameValidationRegex();
                if (!regex.IsMatch(Name))
                {
                    NameError = "Name contains invalid characters.";
                }
                else
                {
                    NameError = string.Empty;
                }
            }
        }


        private void ValidatePrice()
        {
            var regex = PriceFormatRegex();

            if (!regex.IsMatch(Price.ToString()))
            {
                PriceError = "Invalid price format.";
            }
            else if (Price <= 0)
            {
                PriceError = "Price must be greater than 0.";
            }
            else if (Price > 1000000)
            {
                PriceError = "Price is too high.";
            }
            else
            {
                PriceError = string.Empty;
            }
        }


        private void ValidateQuantity()
        {
            var regex = QuantityFormatRegex();

            if (!regex.IsMatch(Quantity.ToString()))
            {
                QuantityError = "Invalid quantity format.";
            }
            else if (Quantity <= 0)
            {
                QuantityError = "Quantity must be greater than 0.";
            }
            else if (Quantity > 1000)
            {
                QuantityError = "Quantity is too high.";
            }
            else
            {
                QuantityError = string.Empty;
            }
        }


        private void ValidateDiscount()
        {
            var regex = DiscountFormatRegex();

            if (!regex.IsMatch(Discount.ToString()))
            {
                DiscountError = "Invalid discount format.";
            }
            else if (Discount < 0)
            {
                DiscountError = "Discount cannot be negative.";
            }
            else if (Discount > Price)
            {
                DiscountError = "Discount cannot exceed price.";
            }
            else
            {
                DiscountError = string.Empty;
            }
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

        [RelayCommand]
        public void Clear()
        {
            Products.Clear();
            AllProductTotalPrice = 0;
            TotalDiscountAmount = 0;
        }

        private bool CanAddProduct()
        {
            ValidateName();
            ValidatePrice();
            ValidateQuantity();
            ValidateDiscount();

            // Check if product with the same name already exists
            if (Products.Any(p => p.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)))
            {
                NameError = "Product already exists.";
                return false;
            }

            return string.IsNullOrEmpty(NameError)
                && string.IsNullOrEmpty(PriceError)
                && string.IsNullOrEmpty(QuantityError)
                && string.IsNullOrEmpty(DiscountError);
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"^[a-zA-Z0-9\s\-]+$")]
        private static partial System.Text.RegularExpressions.Regex NameValidationRegex();
        [System.Text.RegularExpressions.GeneratedRegex(@"^\d+(\.\d{0,2})?$")]
        private static partial System.Text.RegularExpressions.Regex PriceFormatRegex();
        [System.Text.RegularExpressions.GeneratedRegex(@"^\d+(\.\d{0,2})?$")]
        private static partial System.Text.RegularExpressions.Regex DiscountFormatRegex();
        [System.Text.RegularExpressions.GeneratedRegex(@"^\d+$")]
        private static partial System.Text.RegularExpressions.Regex QuantityFormatRegex();
    }
}
