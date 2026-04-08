using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZedCars.Database;
using ZedCars.Models;

namespace ZedCars.Controllers
{
    public class HomeController : Controller
    {
        private List<Car> GetCarsFromDb()
        {
            var cars = new List<Car>();
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader("SELECT CarId, Brand, Model, Year, Price, FuelType, Transmission, Description, ImageUrl, StockQuantity FROM Cars WHERE IsActive = TRUE"))
                {
                    while (reader.Read())
                    {
                        cars.Add(new Car
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
                            StockQuantity = reader.GetInt32("StockQuantity")
                        });
                    }
                }
            }
            catch { }
            return cars;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Message = "Welcome to ZedCars!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var all = GetCarsFromDb();
            all.Sort((a, b) => b.Price.CompareTo(a.Price));
            return View(all.GetRange(0, Math.Min(3, all.Count)));
        }

        public IActionResult About()
        {
            ViewBag.Message = "About ZedCars";
            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Contact Us";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactMessage model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string query = string.Format(
                "INSERT INTO ContactMessages (Name, Email, Phone, Subject, Message) VALUES ('{0}','{1}','{2}','{3}','{4}')",
                model.Name.Replace("'", "''"),
                model.Email.Replace("'", "''"),
                (model.Phone ?? string.Empty).Replace("'", "''"),
                (model.Subject ?? string.Empty).Replace("'", "''"),
                model.Message.Replace("'", "''")
            );
            Database.DatabaseConnection.ExecuteNonQuery(query);

            ViewBag.Success = "Your message has been sent!";
            return View();
        }

        public IActionResult Inventory()
        {
            ViewBag.Message = "Vehicle Inventory";
            return View(GetCarsFromDb());
        }

        public IActionResult VehicleDetail(int id = 0)
        {
            ViewBag.Message = "Vehicle Details";
            Car? car = null;
            try
            {
                using (var reader = DatabaseConnection.ExecuteReader(
                    "SELECT CarId, Brand, Model, Year, Price, FuelType, Transmission, Description, ImageUrl, StockQuantity FROM Cars WHERE CarId = " + id + " AND IsActive = TRUE"))
                {
                    if (reader.Read())
                    {
                        car = new Car
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
                            StockQuantity = reader.GetInt32("StockQuantity")
                        };
                    }
                }
            }
            catch { }
            if (car == null) return NotFound();
            return View(car);
        }

        public IActionResult Direct()
        {
            ViewBag.Message = "Direct View Test";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return View();
        }

        // Simple test action that returns plain text
        public ContentResult Test()
        {
            return Content("Home controller is working! Time: " + DateTime.Now);
        }

        // Simple test action that returns JSON
        public JsonResult Status()
        {
            return Json(new
            {
                Status     = "OK",
                Controller = "Home",
                Time       = DateTime.Now.ToString()
            });
        }
    }
}
