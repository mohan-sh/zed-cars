using System;
using System.Web.Mvc;

namespace ZedCars.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ZedCars!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return View();
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
            return View();
        }
        
        public ActionResult VehicleDetail(int id = 4)
        {
            ViewBag.Message = "Vehicle Details";
            ViewBag.VehicleId = id;
            return View();
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
