using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZedCars.Database;

namespace ZedCars.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
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
        public IActionResult Inventory()
        {
            var cars = new List<ZedCars.Models.Car>();
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT CarId, Brand, Model, Year, Price, FuelType, Transmission, ImageUrl, StockQuantity FROM Cars WHERE IsActive=TRUE ORDER BY CarId DESC"))
                {
                    while (reader.Read())
                        cars.Add(new ZedCars.Models.Car
                        {
                            CarId         = reader.GetInt32("CarId"),
                            Brand         = reader["Brand"].ToString() ?? string.Empty,
                            Model         = reader["Model"].ToString() ?? string.Empty,
                            Year          = reader["Year"].ToString() ?? string.Empty,
                            Price         = reader.GetDecimal("Price"),
                            FuelType      = reader["FuelType"].ToString() ?? string.Empty,
                            Transmission  = reader["Transmission"].ToString() ?? string.Empty,
                            ImageUrl      = reader["ImageUrl"].ToString() ?? string.Empty,
                            StockQuantity = reader.GetInt32("StockQuantity")
                        });
                }
            }
            catch { }
            return View(cars);
        }

        // GET: /Admin/AddVehicle
        public IActionResult AddVehicle()
        {
            return View();
        }

        // POST: /Admin/AddVehicle
        [HttpPost]
        public IActionResult AddVehicle(ZedCars.Models.Car car)
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
        public IActionResult EditVehicle(int id)
        {
            ZedCars.Models.Car? car = null;
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT CarId, Brand, Model, Year, Price, FuelType, Transmission, Description, ImageUrl, Color, Mileage, StockQuantity FROM Cars WHERE CarId=" + id + " AND IsActive=TRUE"))
                {
                    if (reader.Read())
                        car = new ZedCars.Models.Car
                        {
                            CarId         = reader.GetInt32("CarId"),
                            Brand         = reader["Brand"].ToString() ?? string.Empty,
                            Model         = reader["Model"].ToString() ?? string.Empty,
                            Year          = reader["Year"].ToString() ?? string.Empty,
                            Price         = reader.GetDecimal("Price"),
                            FuelType      = reader["FuelType"].ToString() ?? string.Empty,
                            Transmission  = reader["Transmission"].ToString() ?? string.Empty,
                            Description   = reader["Description"].ToString() ?? string.Empty,
                            ImageUrl      = reader["ImageUrl"].ToString() ?? string.Empty,
                            Color         = reader["Color"].ToString() ?? string.Empty,
                            Mileage       = reader["Mileage"] == DBNull.Value ? (int?)null : reader.GetInt32("Mileage"),
                            StockQuantity = reader.GetInt32("StockQuantity")
                        };
                }
            }
            catch { }
            if (car == null) return NotFound();
            return View(car);
        }

        // POST: /Admin/EditVehicle/5
        [HttpPost]
        public IActionResult EditVehicle(ZedCars.Models.Car car)
        {
            try
            {
                string sql = string.Format(
                    "UPDATE Cars SET Brand='{0}', Model='{1}', Year='{2}', Price={3}, FuelType='{4}', Transmission='{5}', Description='{6}', ImageUrl='{7}', Color='{8}', Mileage={9}, StockQuantity={10} WHERE CarId={11}",
                    car.Brand, car.Model, car.Year, car.Price, car.FuelType, car.Transmission,
                    (car.Description ?? string.Empty).Replace("'", "''"),
                    (car.ImageUrl ?? string.Empty).Replace("'", "''"),
                    (car.Color ?? string.Empty).Replace("'", "''"),
                    car.Mileage.HasValue ? car.Mileage.Value.ToString() : "0",
                    car.StockQuantity, car.CarId);
                DatabaseConnection.ExecuteNonQuery(sql);
                TempData["SuccessMessage"] = "Vehicle updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to update vehicle: " + ex.Message;
            }
            return RedirectToAction("Inventory");
        }

        // GET: /Admin/DeleteVehicle/5
        public IActionResult DeleteVehicle(int? id)
        {
            if (!id.HasValue) return RedirectToAction("Inventory");
            ZedCars.Models.Car? car = null;
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT CarId, Brand, Model, Year, Price FROM Cars WHERE CarId=" + id.Value + " AND IsActive=TRUE"))
                {
                    if (reader.Read())
                        car = new ZedCars.Models.Car
                        {
                            CarId = reader.GetInt32("CarId"),
                            Brand = reader["Brand"].ToString() ?? string.Empty,
                            Model = reader["Model"].ToString() ?? string.Empty,
                            Year  = reader["Year"].ToString() ?? string.Empty,
                            Price = reader.GetDecimal("Price")
                        };
                }
            }
            catch { }
            if (car == null) return NotFound();
            return View(car);
        }

        // POST: /Admin/DeleteVehicle/5
        [HttpPost, ActionName("DeleteVehicle")]
        public IActionResult DeleteVehicleConfirmed(int id)
        {
            try
            {
                int rows = DatabaseConnection.ExecuteNonQuery("DELETE FROM Cars WHERE CarId=" + id);
                TempData["SuccessMessage"] = rows > 0 ? "Vehicle deleted successfully!" : "No vehicle found with that ID.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to delete vehicle: " + ex.Message;
            }
            return RedirectToAction("Inventory");
        }

        // GET: /Admin/ManageUsers
        public IActionResult ManageUsers()
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
                            Username    = reader["Username"].ToString() ?? string.Empty,
                            FullName    = reader["FullName"].ToString() ?? string.Empty,
                            Role        = reader["Role"].ToString() ?? string.Empty,
                            IsActive    = reader.GetBoolean("IsActive"),
                            CreatedDate = reader["CreatedDate"].ToString() ?? string.Empty
                        });
                }
            }
            catch { }
            return View(users);
        }

        // GET: /Admin/Reports
        public IActionResult Reports()
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
                        brandStats.Add(new object[] { reader["Brand"].ToString() ?? string.Empty, reader["Count"] });
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
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UserListItem
    {
        public string Username    { get; set; } = string.Empty;
        public string FullName    { get; set; } = string.Empty;
        public string Role        { get; set; } = string.Empty;
        public bool   IsActive    { get; set; }
        public string CreatedDate { get; set; } = string.Empty;
    }
}
