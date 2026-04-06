using System;

namespace ZedCars.Database
{
    public static class DataSeeder
    {
        public static void SeedData()
        {
            try
            {
                SeedAdmins();
                SeedCars();
                SeedAccessories();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Seeding error: " + ex.Message);
            }
        }
        
        private static void SeedAdmins()
        {
            // Check if admins already exist
            var count = DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Admins");
            if (Convert.ToInt32(count) > 0) return;
            
            string insertAdmins = @"
                INSERT INTO Admins (Username, Password, FullName, Email, Department, Role, PhoneNumber, IsActive, Permissions) 
                VALUES 
                ('admin1', 'password123', 'John Smith', 'john.smith@zedcars.com', 'Inventory', 'Manager', '555-0101', TRUE, 'inventory,sales,reports'),
                ('admin2', 'password123', 'Sarah Johnson', 'sarah.johnson@zedcars.com', 'Sales', 'Senior', '555-0102', TRUE, 'sales,reports'),
                ('superadmin', 'admin123', 'Super Administrator', 'super@zedcars.com', 'Management', 'SuperAdmin', '555-0110', TRUE, 'all,inventory,sales,reports,admin')";
            
            DatabaseConnection.ExecuteNonQuery(insertAdmins);
        }
        
        private static void SeedCars()
        {
            // Check if cars already exist
            var count = DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Cars");
            if (Convert.ToInt32(count) > 0) return;
            
            string insertCars = @"
                INSERT INTO Cars (Model, Brand, Variant, Price, StockQuantity, Color, Year, FuelType, Transmission, Mileage, Description) 
                VALUES 
                ('Camry', 'Toyota', 'LE', 24000.00, 15, 'White', '2024', 'Gasoline', 'Automatic', 28, 'Reliable mid-size sedan'),
                ('Accord', 'Honda', 'LX', 25000.00, 12, 'Black', '2024', 'Gasoline', 'Automatic', 30, 'Popular family sedan'),
                ('Mustang', 'Ford', 'GT', 35000.00, 8, 'Red', '2024', 'Gasoline', 'Manual', 20, 'Iconic sports car'),
                ('3 Series', 'BMW', '320i', 40000.00, 10, 'Blue', '2024', 'Gasoline', 'Automatic', 26, 'Luxury compact sedan'),
                ('C-Class', 'Mercedes', 'C300', 42000.00, 7, 'Silver', '2024', 'Gasoline', 'Automatic', 25, 'Premium luxury sedan')";
            
            DatabaseConnection.ExecuteNonQuery(insertCars);
        }
        
        private static void SeedAccessories()
        {
            // Check if accessories already exist
            var count = DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Accessories");
            if (Convert.ToInt32(count) > 0) return;
            
            string insertAccessories = @"
                INSERT INTO Accessories (Name, Category, Price, StockQuantity, Description, PartNumber, Manufacturer) 
                VALUES 
                ('Floor Mats', 'Interior', 89.99, 50, 'All-weather floor protection', 'FM-001', 'WeatherTech'),
                ('Seat Covers', 'Interior', 149.99, 30, 'Premium leather seat covers', 'SC-002', 'Covercraft'),
                ('GPS Navigation', 'Electronics', 399.99, 20, 'Touch screen GPS system', 'GPS-201', 'Garmin'),
                ('Backup Camera', 'Electronics', 199.99, 30, 'Rear view safety camera', 'BC-202', 'Pioneer'),
                ('Body Kit', 'Exterior', 899.99, 10, 'Aerodynamic body enhancement', 'BK-101', 'Duraflex')";
            
            DatabaseConnection.ExecuteNonQuery(insertAccessories);
        }
    }
}
