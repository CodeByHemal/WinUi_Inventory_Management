using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi_Inventory_Management
{
    internal partial class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\..\.."));
            string dbFilePath = Path.Combine(projectRoot, "Database", "RetailRythm.mdf");

            string connectionTemplate = ConfigurationManager
                .ConnectionStrings["SampleDatabaseWalkthrough.Properties.Settings.SampleDatabaseConnectionString"]
                .ConnectionString;

            string connectionString = connectionTemplate.Replace("{DB_PATH}", dbFilePath);

            // Auto-drop if .mdf is missing but LocalDB still has the database
            if (!File.Exists(dbFilePath))
            {
                try
                {
                    string masterConnection = ConfigurationManager
                        .ConnectionStrings["RetailRythmDBMaster"].ConnectionString;

                    using var conn = new SqlConnection(masterConnection);
                    conn.Open();

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
                IF DB_ID('RetailRythmDB') IS NOT NULL
                BEGIN
                    ALTER DATABASE RetailRythmDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE RetailRythmDB;
                END";
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    // do nothing
                }
            }

            optionsBuilder.UseSqlServer(connectionString);
        }

    }
    class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
