using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ZedCars.Database;

namespace ZedCars.Controllers
{
    public class AccountController : Controller
    {
        private UserInfo? GetUserFromDb(string username, string password)
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
                                    Username = reader["Username"].ToString() ?? string.Empty,
                                    Password = reader["Password"].ToString() ?? string.Empty,
                                    FullName = reader["FullName"].ToString() ?? string.Empty,
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
                                    Username = reader["Username"].ToString() ?? string.Empty,
                                    Password = reader["Password"].ToString() ?? string.Empty,
                                    FullName = reader["FullName"].ToString() ?? string.Empty,
                                    Role     = reader["Role"].ToString() ?? string.Empty
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

        private UserInfo? GetUserByUsername(string username)
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
                                    Username = reader["Username"].ToString() ?? string.Empty,
                                    Password = reader["Password"].ToString() ?? string.Empty,
                                    FullName = reader["FullName"].ToString() ?? string.Empty,
                                    Role     = reader["Role"].ToString() ?? string.Empty
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
        public IActionResult Landing()
        {
            if (User.Identity?.IsAuthenticated == true)
                return User.IsInRole("Admin")
                    ? RedirectToAction("Dashboard", "Admin")
                    : RedirectToAction("Index", "Home");
            return View();
        }

        // GET: /Account/Login  (User login)
        public IActionResult Login()
        {
            return RedirectToAction("Landing");
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool? remember)
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

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName)
                };
                var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = remember ?? false,
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Home");
            }

            TempData["UserError"] = "Invalid username or password.";
            TempData["ActiveTab"] = "user";
            return RedirectToAction("Landing");
        }

        // GET: /Account/AdminLogin
        public IActionResult AdminLogin()
        {
            return RedirectToAction("Landing");
        }

        // POST: /Account/AdminLogin
        [HttpPost]
        public async Task<IActionResult> AdminLogin(string username, string password, bool? remember)
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName)
                };
                var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = remember ?? false,
                    ExpiresUtc   = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Dashboard", "Admin");
            }

            TempData["AdminError"] = "Invalid admin credentials.";
            TempData["ActiveTab"] = "admin";
            return RedirectToAction("Landing");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Landing", "Account");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string fullName, string email, string username, string password)
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
        public IActionResult UserProfile()
        {
            var user = GetUserByUsername(User.Identity?.Name ?? string.Empty);
            if (user != null) return View(user);
            return RedirectToAction("Login");
        }
    }

    public class UserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role     { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
