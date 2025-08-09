using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

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

            // ✅ Start LocalDB silently only if not running
            try
            {
                var checkPsi = new ProcessStartInfo
                {
                    FileName = "sqllocaldb",
                    Arguments = "info MSSQLLocalDB",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using var checkProcess = Process.Start(checkPsi);
                string output = checkProcess.StandardOutput.ReadToEnd();
                checkProcess.WaitForExit();

                if (!output.Contains("Running"))
                {
                    var startPsi = new ProcessStartInfo
                    {
                        FileName = "sqllocaldb",
                        Arguments = "start MSSQLLocalDB",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    Process.Start(startPsi);
                }
            }
            catch
            {
                // Ignore errors if LocalDB is missing or fails to start
            }

            // ✅ Drop orphaned DB if MDF is missing
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
                catch (Exception ex)
                {
                    Console.WriteLine("⚠️ Failed to clean orphan DB: " + ex.Message);
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
        public string ImageName { get; set; }
    }
}
