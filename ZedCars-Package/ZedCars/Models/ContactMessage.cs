using System.ComponentModel.DataAnnotations;

namespace ZedCars.Models
{
    public class ContactMessage
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
