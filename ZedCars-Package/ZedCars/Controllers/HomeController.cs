using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZedCars.Models;
using ZedCars.Database;

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
                            Brand         = reader["Brand"].ToString(),
                            Model         = reader["Model"].ToString(),
                            Year          = reader["Year"].ToString(),
                            Price         = reader.GetDecimal("Price"),
                            FuelType      = reader["FuelType"].ToString(),
                            Transmission  = reader["Transmission"].ToString(),
                            Description   = reader["Description"].ToString(),
                            ImageUrl      = reader["ImageUrl"].ToString(),
                            StockQuantity = reader.GetInt32("StockQuantity")
                        });
                    }
                }
            }
            catch { }
            return cars;
        }
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ZedCars!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var all = GetCarsFromDb();
            all.Sort((a, b) => b.Price.CompareTo(a.Price));
            return View(all.GetRange(0, Math.Min(3, all.Count)));
        }

        public ActionResult About()
        {
            ViewBag.Message = "About ZedCars";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";
            return View();
        }
        
        public ActionResult Inventory()
        {
            ViewBag.Message = "Vehicle Inventory";
            return View(GetCarsFromDb());
        }
        
        public ActionResult VehicleDetail(int id = 0)
        {
            ViewBag.Message = "Vehicle Details";
            Car car = null;
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
                            Brand         = reader["Brand"].ToString(),
                            Model         = reader["Model"].ToString(),
                            Year          = reader["Year"].ToString(),
                            Price         = reader.GetDecimal("Price"),
                            FuelType      = reader["FuelType"].ToString(),
                            Transmission  = reader["Transmission"].ToString(),
                            Description   = reader["Description"].ToString(),
                            ImageUrl      = reader["ImageUrl"].ToString(),
                            StockQuantity = reader.GetInt32("StockQuantity")
                        };
                    }
                }
            }
            catch { }
            if (car == null) return HttpNotFound();
            return View(car);
        }
        
        public ActionResult Direct()
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
                Status = "OK", 
                Controller = "Home", 
                Time = DateTime.Now.ToString() 
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
