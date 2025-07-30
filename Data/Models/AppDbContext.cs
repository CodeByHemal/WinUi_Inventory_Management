using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace WinUi_Inventory_Management.Data.Models
{
    internal class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=RetailRythm.db");
        }

       }
}
