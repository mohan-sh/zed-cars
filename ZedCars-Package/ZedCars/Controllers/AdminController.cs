using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZedCars.Database;

namespace ZedCars.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Demo vehicles for testing
        private static readonly List<Vehicle> Vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "Toyota", Model = "Camry", Year = 2023, Price = 25000, Status = "Available" },
            new Vehicle { Id = 2, Brand = "Honda", Model = "CR-V", Year = 2023, Price = 32000, Status = "Available" },
            new Vehicle { Id = 3, Brand = "Ford", Model = "F-150", Year = 2022, Price = 45000, Status = "Reserved" },
            new Vehicle { Id = 4, Brand = "BMW", Model = "5 Series", Year = 2023, Price = 55000, Status = "Available" },
            new Vehicle { Id = 5, Brand = "Tesla", Model = "Model 3", Year = 2023, Price = 60000, Status = "Sold" },
            new Vehicle { Id = 6, Brand = "Mercedes", Model = "GLE", Year = 2022, Price = 65000, Status = "Available" }
        };

        // GET: /Admin/Dashboard
        public ActionResult Dashboard()
        {
            try
            {
                ViewBag.TotalVehicles  = DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Cars WHERE IsActive=TRUE") ?? 0;
                ViewBag.TotalBrands    = DatabaseConnection.ExecuteScalar("SELECT COUNT(DISTINCT Brand) FROM Cars WHERE IsActive=TRUE") ?? 0;
                ViewBag.TotalUsers     = DatabaseConnection.ExecuteScalar("SELECT (SELECT COUNT(*) FROM Users) + (SELECT COUNT(*) FROM Admins)") ?? 0;
                ViewBag.InventoryValue = DatabaseConnection.ExecuteScalar("SELECT IFNULL(SUM(Price * StockQuantity),0) FROM Cars WHERE IsActive=TRUE") ?? 0;

                // Recent cars added
                var recentCars = new List<string>();
                using (var reader = DatabaseConnection.ExecuteReader("SELECT Brand, Model, CreatedDate FROM Cars WHERE IsActive=TRUE ORDER BY CreatedDate DESC LIMIT 5"))
                    while (reader.Read())
                        recentCars.Add(reader["Brand"] + " " + reader["Model"] + " (added " + Convert.ToDateTime(reader["CreatedDate"]).ToString("MMM dd") + ")");
                ViewBag.RecentCars = recentCars;
            }
            catch
            {
                ViewBag.TotalVehicles = 0; ViewBag.TotalBrands = 0;
                ViewBag.TotalUsers = 0; ViewBag.InventoryValue = 0;
                ViewBag.RecentCars = new List<string>();
            }
            return View();
        }

        // GET: /Admin/Inventory
        public ActionResult Inventory()
        {
            ViewBag.Vehicles = Vehicles;
            return View();
        }

        // GET: /Admin/AddVehicle
        public ActionResult AddVehicle()
        {
            return View();
        }

        // POST: /Admin/AddVehicle
        [HttpPost]
        public ActionResult AddVehicle(ZedCars.Models.Car car)
        {
            try
            {
                string sql = string.Format(
                    "INSERT INTO Cars (Brand, Model, Year, Price, FuelType, Transmission, Description, ImageUrl, Color, Mileage, StockQuantity, IsActive, CreatedDate) " +
                    "VALUES ('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', {9}, 1, TRUE, NOW())",
                    car.Brand, car.Model, car.Year, car.Price, car.FuelType, car.Transmission,
                    car.Description, car.ImageUrl, car.Color,
                    car.Mileage.HasValue ? car.Mileage.Value.ToString() : "0");

                ZedCars.Database.DatabaseConnection.ExecuteNonQuery(sql);
                TempData["SuccessMessage"] = "Vehicle added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to add vehicle: " + ex.Message;
            }
            return RedirectToAction("Inventory", "Home");
        }

        // GET: /Admin/EditVehicle/5
        public ActionResult EditVehicle(int id)
        {
            var vehicle = Vehicles.Find(v => v.Id == id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            
            return View(vehicle);
        }

        // POST: /Admin/EditVehicle/5
        [HttpPost]
        public ActionResult EditVehicle(Vehicle vehicle)
        {
            // In a real application, you would update the vehicle in a database
            var index = Vehicles.FindIndex(v => v.Id == vehicle.Id);
            if (index >= 0)
            {
                Vehicles[index] = vehicle;
                TempData["SuccessMessage"] = "Vehicle updated successfully!";
            }
            
            return RedirectToAction("Inventory");
        }

        // GET: /Admin/DeleteVehicle/5
        public ActionResult DeleteVehicle(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Inventory");
            var vehicle = Vehicles.Find(v => v.Id == id.Value);
            if (vehicle == null) return HttpNotFound();
            return View(vehicle);
        }

        // POST: /Admin/DeleteVehicle/5
        [HttpPost, ActionName("DeleteVehicle")]
        public ActionResult DeleteVehicleConfirmed(int id)
        {
            // In a real application, you would delete the vehicle from a database
            var vehicle = Vehicles.Find(v => v.Id == id);
            if (vehicle != null)
            {
                Vehicles.Remove(vehicle);
                TempData["SuccessMessage"] = "Vehicle deleted successfully!";
            }
            
            return RedirectToAction("Inventory");
        }
        
        // GET: /Admin/ManageUsers
        public ActionResult ManageUsers()
        {
            var users = new List<UserListItem>();
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT Username, FullName, Role, IsActive, CreatedDate FROM Users " +
                    "UNION ALL " +
                    "SELECT Username, FullName, 'Admin' AS Role, IsActive, CreatedDate FROM Admins " +
                    "ORDER BY CreatedDate DESC"))
                {
                    while (reader.Read())
                        users.Add(new UserListItem
                        {
                            Username    = reader["Username"].ToString(),
                            FullName    = reader["FullName"].ToString(),
                            Role        = reader["Role"].ToString(),
                            IsActive    = reader.GetBoolean("IsActive"),
                            CreatedDate = reader["CreatedDate"].ToString()
                        });
                }
            }
            catch { }
            return View(users);
        }
        
        // GET: /Admin/Reports
        public ActionResult Reports()
        {
            try
            {
                ViewBag.TotalVehicles   = DatabaseConnection.ExecuteScalar("SELECT COUNT(*) FROM Cars WHERE IsActive=TRUE") ?? 0;
                ViewBag.TotalBrands     = DatabaseConnection.ExecuteScalar("SELECT COUNT(DISTINCT Brand) FROM Cars WHERE IsActive=TRUE") ?? 0;
                ViewBag.InventoryValue  = DatabaseConnection.ExecuteScalar("SELECT IFNULL(SUM(Price * StockQuantity),0) FROM Cars WHERE IsActive=TRUE") ?? 0;
                ViewBag.TotalUsers      = DatabaseConnection.ExecuteScalar("SELECT (SELECT COUNT(*) FROM Users) + (SELECT COUNT(*) FROM Admins)") ?? 0;

                // Brand breakdown
                var brandStats = new List<object[]>();
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT Brand, COUNT(*) as Count FROM Cars WHERE IsActive=TRUE GROUP BY Brand ORDER BY Count DESC"))
                    while (reader.Read())
                        brandStats.Add(new object[] { reader["Brand"].ToString(), reader["Count"] });
                ViewBag.BrandStats = brandStats;

                // Recent cars
                var recentCars = new List<object[]>();
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT Brand, Model, Price, CreatedDate FROM Cars WHERE IsActive=TRUE ORDER BY CreatedDate DESC LIMIT 5"))
                    while (reader.Read())
                        recentCars.Add(new object[] { reader["Brand"] + " " + reader["Model"], reader["Price"], reader["CreatedDate"] });
                ViewBag.RecentCars = recentCars;
            }
            catch
            {
                ViewBag.TotalVehicles = 0; ViewBag.TotalBrands = 0;
                ViewBag.InventoryValue = 0; ViewBag.TotalUsers = 0;
                ViewBag.BrandStats = new List<object[]>();
                ViewBag.RecentCars = new List<object[]>();
            }
            return View();
        }
    }

    public class Vehicle
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }

    public class UserListItem
    {
        public string Username    { get; set; }
        public string FullName    { get; set; }
        public string Role        { get; set; }
        public bool   IsActive    { get; set; }
        public string CreatedDate { get; set; }
    }
}
