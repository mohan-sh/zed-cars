using System;
using System.Web.Mvc;

namespace ZedCars.Controllers
{
    public class TestController : Controller
    {
        // Simple test action that doesn't use database
        public ActionResult Index()
        {
            ViewBag.Message = "ZedCars Test Page - Application is Running!";
            ViewBag.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.ServerInfo = Request.Url?.Host + ":" + Request.Url?.Port;
            
            return View();
        }
        
        // Simple JSON response for AJAX testing
        public JsonResult Status()
        {
            return Json(new 
            { 
                Status = "OK", 
                Message = "ZedCars application is running",
                Time = DateTime.Now,
                Server = Request.Url?.Host + ":" + Request.Url?.Port
            }, JsonRequestBehavior.AllowGet);
        }
        
        // Simple text response
        public ActionResult Ping()
        {
            return Content("PONG - ZedCars is alive at " + DateTime.Now);
        }
    }
}
