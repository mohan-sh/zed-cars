using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ZedCars.Database
{
    public static class DatabaseConnection
    {
        private static string GetConnectionString()
        {
            // Try multiple connection string names for flexibility
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["ZedCarsDB"] 
                                         ?? ConfigurationManager.ConnectionStrings["DefaultConnection"];
            
            if (connectionStringSettings != null)
            {
                return connectionStringSettings.ConnectionString;
            }
            
            // Fallback connection string if not found in config
            return "Server=localhost;Database=zoomcars_inventory;Uid=zoomcars_user;Pwd=admin123;Port=3306;";
        }
        
        public static MySqlConnection GetConnection()
        {
            try
            {
                string connectionString = GetConnectionString();
                return new MySqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database connection error: " + ex.Message);
                throw;
            }
        }
        
        public static object ExecuteScalar(string query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteScalar();
                }
            }
        }
        
        public static int ExecuteNonQuery(string query)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }
        
        public static MySqlDataReader ExecuteReader(string query)
        {
            var connection = GetConnection();
            connection.Open();
            var command = new MySqlCommand(query, connection);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        
        public static bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
