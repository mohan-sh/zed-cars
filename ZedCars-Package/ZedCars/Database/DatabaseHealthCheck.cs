using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace ZedCars.Database
{
    public static class DatabaseHealthCheck
    {
        public static DatabaseStatus CheckDatabaseHealth()
        {
            var status = new DatabaseStatus();
            
            try
            {
                // Test basic connection
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    status.IsConnected = connection.State == ConnectionState.Open;
                    status.ServerVersion = connection.ServerVersion;
                }
                
                if (status.IsConnected)
                {
                    // Check table existence and record counts
                    status.TablesExist = CheckTablesExist();
                    status.CarCount = GetRecordCount("Cars");
                    status.AdminCount = GetRecordCount("Admins");
                    status.AccessoryCount = GetRecordCount("Accessories");
                    status.HasData = status.CarCount > 0 && status.AdminCount > 0;
                }
            }
            catch (Exception ex)
            {
                status.IsConnected = false;
                status.ErrorMessage = ex.Message;
            }
            
            return status;
        }
        
        private static bool CheckTablesExist()
        {
            try
            {
                var tables = new[] { "Cars", "Admins", "Accessories" };
                foreach (var table in tables)
                {
                    var query = $"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = '{table}'";
                    var result = DatabaseConnection.ExecuteScalar(query);
                    if (Convert.ToInt32(result) == 0)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private static int GetRecordCount(string tableName)
        {
            try
            {
                var result = DatabaseConnection.ExecuteScalar($"SELECT COUNT(*) FROM {tableName}");
                return Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }
        
        public static void SeedDataIfEmpty()
        {
            try
            {
                var status = CheckDatabaseHealth();
                if (status.IsConnected && status.TablesExist && !status.HasData)
                {
                    DataSeeder.SeedData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error seeding data: " + ex.Message);
            }
        }
    }
    
    public class DatabaseStatus
    {
        public bool IsConnected { get; set; }
        public bool TablesExist { get; set; }
        public bool HasData { get; set; }
        public string ServerVersion { get; set; }
        public string ErrorMessage { get; set; }
        public int CarCount { get; set; }
        public int AdminCount { get; set; }
        public int AccessoryCount { get; set; }
        
        public string GetStatusMessage()
        {
            if (!IsConnected)
                return $"❌ Database connection failed: {ErrorMessage}";
            
            if (!TablesExist)
                return "⚠️ Database connected but tables are missing";
            
            if (!HasData)
                return "⚠️ Database connected but no data found";
            
            return $"✅ Database healthy - Cars: {CarCount}, Admins: {AdminCount}, Accessories: {AccessoryCount}";
        }
    }
}
