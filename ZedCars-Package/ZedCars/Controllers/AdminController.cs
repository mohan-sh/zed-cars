using System;
using System.Collections.Generic;
using System.Web.Mvc;

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
            ViewBag.Vehicles = Vehicles;
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
        public ActionResult AddVehicle(Vehicle vehicle)
        {
            // In a real application, you would add the vehicle to a database
            vehicle.Id = Vehicles.Count + 1;
            Vehicles.Add(vehicle);
            
            TempData["SuccessMessage"] = "Vehicle added successfully!";
            return RedirectToAction("Inventory");
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
        public ActionResult DeleteVehicle(int id)
        {
            var vehicle = Vehicles.Find(v => v.Id == id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            
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
            // In a real application, you would get users from a database
            return View();
        }
        
        // GET: /Admin/Reports
        public ActionResult Reports()
        {
            // In a real application, you would generate reports from database data
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
}
