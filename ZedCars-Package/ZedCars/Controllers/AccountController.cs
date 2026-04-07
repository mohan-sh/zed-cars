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

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password, bool? remember)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Username and password are required.";
                return View();
            }

            var user = GetUserFromDb(username, password);
            if (user != null)
            {
                var ticket = new FormsAuthenticationTicket(
                    1, username, DateTime.Now, DateTime.Now.AddMinutes(30),
                    remember ?? false, user.Role, FormsAuthentication.FormsCookiePath);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                Response.Cookies.Add(cookie);
                Session["UserInfo"] = user;

                return user.Role == "Admin"
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Invalid username or password.";
            return View();
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
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
                        "INSERT INTO Users (Username, Password, FullName, Role, IsActive) VALUES (@u, @p, @f, 'Customer', TRUE)",
                        connection))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", password);
                        cmd.Parameters.AddWithValue("@f", fullName);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Registration successful! Please sign in.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Register error: " + ex.Message);
                TempData["ErrorMessage"] = "Registration failed. Username may already exist.";
                return View();
            }
            return RedirectToAction("Login");
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
