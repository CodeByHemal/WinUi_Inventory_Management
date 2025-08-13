using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi_Inventory_Management.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Foreign key
        public int OrderId { get; set; }
        public Order Order { get; set; } // Navigation property

        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int ItemQuantity { get; set; }
        public decimal Total { get; set; }
        public decimal Discount { get; set; }
    }
}
