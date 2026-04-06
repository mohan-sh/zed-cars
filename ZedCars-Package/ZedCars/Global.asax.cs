using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZedCars
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                RegisterGlobalFilters(GlobalFilters.Filters);
                RegisterRoutes(RouteTable.Routes);
                
                // Use Razor view engine for .cshtml files
                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(new RazorViewEngine());
                
                System.Diagnostics.Debug.WriteLine("ZedCars application started successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Application start error: " + ex.Message);
                throw;
            }
        }
        
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            System.Diagnostics.Debug.WriteLine("Application error: " + exception?.Message);
        }
    }
}
