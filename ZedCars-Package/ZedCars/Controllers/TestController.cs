using Microsoft.AspNetCore.Mvc;

namespace ZedCars.Controllers
{
    public class TestController : Controller
    {
        // Simple test action that doesn't use database
        public IActionResult Index()
        {
            ViewBag.Message = "ZedCars Test Page - Application is Running!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.ServerInfo = Request.Host.ToString();

            return View();
        }

        // Simple JSON response for AJAX testing
        public JsonResult Status()
        {
            return Json(new
            {
                Status  = "OK",
                Message = "ZedCars application is running",
                Time    = DateTime.Now,
                Server  = Request.Host.ToString()
            });
        }

        // Simple text response
        public IActionResult Ping()
        {
            return Content("PONG - ZedCars is alive at " + DateTime.Now);
        }
    }
}
