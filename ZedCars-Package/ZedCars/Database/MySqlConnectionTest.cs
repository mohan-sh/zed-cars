using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace ZedCars.Database
{
    /// <summary>
    /// MySQL Connection Test Class
    /// Use this to verify MySQL connectivity from the application
    /// </summary>
    public static class MySqlConnectionTest
    {
        /// <summary>
        /// Test MySQL connection and return detailed status
        /// </summary>
        /// <returns>Connection test results</returns>
        public static string TestConnection()
        {
            try
            {
                // Get connection string from Web.config
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    return "❌ ERROR: No connection string found in Web.config";
                }

                // Test connection
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Test basic query
                    using (var command = new MySqlCommand("SELECT VERSION() as MySqlVersion, NOW() as CurrentTime", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string version = reader["MySqlVersion"].ToString();
                                string currentTime = reader["CurrentTime"].ToString();
                                
                                return $"✅ SUCCESS: MySQL {version} connected at {currentTime}";
                            }
                        }
                    }
                }
                
                return "✅ SUCCESS: Connection opened but no data returned";
            }
            catch (MySqlException mysqlEx)
            {
                return $"❌ MySQL ERROR: {mysqlEx.Message} (Code: {mysqlEx.Number})";
            }
            catch (Exception ex)
            {
                return $"❌ GENERAL ERROR: {ex.Message}";
            }
        }

        /// <summary>
        /// Test database and tables existence
        /// </summary>
        /// <returns>Database test results</returns>
        public static string TestDatabase()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    return "❌ ERROR: No connection string found";
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Check database
                    using (var command = new MySqlCommand("SELECT DATABASE() as CurrentDB", connection))
                    {
                        var currentDb = command.ExecuteScalar()?.ToString();
                        
                        if (string.IsNullOrEmpty(currentDb))
                        {
                            return "❌ ERROR: No database selected";
                        }
                        
                        // Check tables
                        command.CommandText = "SHOW TABLES";
                        var tables = new System.Collections.Generic.List<string>();
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tables.Add(reader[0].ToString());
                            }
                        }
                        
                        if (tables.Count == 0)
                        {
                            return $"⚠️ WARNING: Database '{currentDb}' exists but no tables found";
                        }
                        
                        return $"✅ SUCCESS: Database '{currentDb}' with {tables.Count} tables: {string.Join(", ", tables)}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ ERROR: {ex.Message}";
            }
        }

        /// <summary>
        /// Test data access (Cars table)
        /// </summary>
        /// <returns>Data access test results</returns>
        public static string TestDataAccess()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
                
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Test Cars table
                    using (var command = new MySqlCommand("SELECT COUNT(*) as CarCount FROM Cars", connection))
                    {
                        var carCount = command.ExecuteScalar();
                        
                        // Test Admins table
                        command.CommandText = "SELECT COUNT(*) as AdminCount FROM Admins";
                        var adminCount = command.ExecuteScalar();
                        
                        // Test Accessories table
                        command.CommandText = "SELECT COUNT(*) as AccessoryCount FROM Accessories";
                        var accessoryCount = command.ExecuteScalar();
                        
                        return $"✅ SUCCESS: Data access working - Cars: {carCount}, Admins: {adminCount}, Accessories: {accessoryCount}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ ERROR: {ex.Message}";
            }
        }

        /// <summary>
        /// Run comprehensive MySQL test
        /// </summary>
        /// <returns>Complete test results</returns>
        public static string RunCompleteTest()
        {
            var results = new System.Text.StringBuilder();
            results.AppendLine("🔍 MySQL Connection Test Results:");
            results.AppendLine("================================");
            results.AppendLine();
            
            results.AppendLine("1. Connection Test:");
            results.AppendLine("   " + TestConnection());
            results.AppendLine();
            
            results.AppendLine("2. Database Test:");
            results.AppendLine("   " + TestDatabase());
            results.AppendLine();
            
            results.AppendLine("3. Data Access Test:");
            results.AppendLine("   " + TestDataAccess());
            results.AppendLine();
            
            results.AppendLine("Test completed at: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            return results.ToString();
        }
    }
}
