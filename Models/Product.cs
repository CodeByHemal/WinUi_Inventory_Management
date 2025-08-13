using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WinUi_Inventory_Management.ViewModels;

namespace WinUi_Inventory_Management.Models
{
    public partial class Product : ObservableObject
    {

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private double price;

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private double discount;

        [ObservableProperty]
        private double totalPrice;
    }

}
