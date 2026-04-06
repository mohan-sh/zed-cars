using System;
using System.ComponentModel.DataAnnotations;

namespace ZedCars.Models
{
    /// <summary>
    /// Model for admin login functionality
    /// Used for authentication in the ZedCars application
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// Admin username for login
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        /// <summary>
        /// Admin password for login
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Remember me option for persistent login
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Error message to display on login failure
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Return URL after successful login
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginModel()
        {
            RememberMe = false;
            ErrorMessage = string.Empty;
            ReturnUrl = string.Empty;
        }

        /// <summary>
        /// Constructor with username and password
        /// </summary>
        /// <param name="username">Admin username</param>
        /// <param name="password">Admin password</param>
        public LoginModel(string username, string password)
        {
            Username = username;
            Password = password;
            RememberMe = false;
            ErrorMessage = string.Empty;
            ReturnUrl = string.Empty;
        }

        /// <summary>
        /// Validates the login model
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        /// <summary>
        /// Clears sensitive data from the model
        /// </summary>
        public void ClearSensitiveData()
        {
            Password = string.Empty;
        }
    }
}
