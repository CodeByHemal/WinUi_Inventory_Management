using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi_Inventory_Management.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string OrderName { get; set; }
        public double TotalPrice { get; set; }
        public double TotalDiscount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    }
}
