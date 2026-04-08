using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZedCars.Database;
using MySql.Data.MySqlClient;

namespace ZedCars.Controllers
{
    public class AccountController : Controller
    {
        private UserInfo GetUserFromDb(string username, string password)
        {
            // First check Admins table
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT Username, Password, FullName, Role FROM Admins WHERE Username=@u AND Password=@p AND IsActive=TRUE",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return new UserInfo
                                {
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Role     = "Admin"
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DB admin login error: " + ex.Message);
            }

            // Then check Users table
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT Username, Password, FullName, Role FROM Users WHERE Username=@u AND Password=@p AND IsActive=TRUE",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return new UserInfo
                                {
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Role     = reader["Role"].ToString()
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DB login error: " + ex.Message);
            }
            return null;
        }

        private UserInfo GetUserByUsername(string username)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT Username, Password, FullName, Role FROM Users WHERE Username=@u AND IsActive=TRUE",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return new UserInfo
                                {
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Role     = reader["Role"].ToString()
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DB user fetch error: " + ex.Message);
            }
            return null;
        }

        // GET: /Account/Landing
        public ActionResult Landing()
        {
            if (Request.IsAuthenticated)
                return User.IsInRole("Admin")
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Index", "Home");
            return View();
        }

        // GET: /Account/Login  (User login)
        public ActionResult Login()
        {
            return RedirectToAction("Landing");
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password, bool? remember)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["UserError"] = "Username and password are required.";
                TempData["ActiveTab"] = "user";
                return RedirectToAction("Landing");
            }

            var user = GetUserFromDb(username, password);
            if (user != null)
            {
                if (user.Role == "Admin")
                {
                    TempData["UserError"] = "Admins must use the Admin Login tab.";
                    TempData["ActiveTab"] = "user";
                    return RedirectToAction("Landing");
                }

                var ticket = new FormsAuthenticationTicket(
                    1, username, DateTime.Now, DateTime.Now.AddMinutes(30),
                    remember ?? false, user.Role, FormsAuthentication.FormsCookiePath);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                Response.Cookies.Add(cookie);
                Session["UserInfo"] = user;

                return RedirectToAction("Index", "Home");
            }

            TempData["UserError"] = "Invalid username or password.";
            TempData["ActiveTab"] = "user";
            return RedirectToAction("Landing");
        }

        // GET: /Account/AdminLogin
        public ActionResult AdminLogin()
        {
            return RedirectToAction("Landing");
        }

        // POST: /Account/AdminLogin
        [HttpPost]
        public ActionResult AdminLogin(string username, string password, bool? remember)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["AdminError"] = "Username and password are required.";
                TempData["ActiveTab"] = "admin";
                return RedirectToAction("Landing");
            }

            var user = GetUserFromDb(username, password);
            if (user != null && user.Role == "Admin")
            {
                var ticket = new FormsAuthenticationTicket(
                    1, username, DateTime.Now, DateTime.Now.AddMinutes(30),
                    remember ?? false, user.Role, FormsAuthentication.FormsCookiePath);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                Response.Cookies.Add(cookie);
                Session["UserInfo"] = user;

                return RedirectToAction("Dashboard", "Admin");
            }

            TempData["AdminError"] = "Invalid admin credentials.";
            TempData["ActiveTab"] = "admin";
            return RedirectToAction("Landing");
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Landing", "Account");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(string fullName, string email, string username, string password)
        {
            try
            {
                using (var connection = DatabaseConnection.GetConnection())
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(
                        "INSERT INTO Users (Username, Password, FullName, Email, Role, IsActive) VALUES (@u, @p, @f, @e, 'Customer', TRUE)",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.Parameters.AddWithValue("@f", fullName);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["RegSuccess"] = "Registration successful! Please sign in.";
                TempData["ActiveTab"] = "user";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Register error: " + ex.Message);
                TempData["RegError"] = "Registration failed. Username may already exist.";
                TempData["ActiveTab"] = "register";
            }
            return RedirectToAction("Landing");
        }

        // GET: /Account/UserProfile
        [Authorize]
        public ActionResult UserProfile()
        {
            var user = GetUserByUsername(User.Identity.Name);
            if (user != null) return View(user);
            return RedirectToAction("Login");
        }
    }

    public class UserInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role     { get; set; }
        public string FullName { get; set; }
    }
}
