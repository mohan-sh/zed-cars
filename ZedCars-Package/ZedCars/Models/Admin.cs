using System;
using System.ComponentModel.DataAnnotations;

namespace ZedCars.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        
        [StringLength(50)]
        public string Role { get; set; }
        
        [StringLength(20)]
        public string PhoneNumber { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        [StringLength(50)]
        public string CreatedBy { get; set; }
        
        [StringLength(500)]
        public string Permissions { get; set; }
        
        public Admin()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}
