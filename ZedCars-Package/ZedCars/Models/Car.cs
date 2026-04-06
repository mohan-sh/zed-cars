using System;
using System.ComponentModel.DataAnnotations;

namespace ZedCars.Models
{
    public class Car
    {
        public int CarId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Model { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Brand { get; set; }
        
        [StringLength(50)]
        public string Variant { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
        
        [StringLength(20)]
        public string Color { get; set; }
        
        [StringLength(4)]
        public string Year { get; set; }
        
        [StringLength(20)]
        public string FuelType { get; set; }
        
        [StringLength(20)]
        public string Transmission { get; set; }
        
        public int? Mileage { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [StringLength(200)]
        public string ImageUrl { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        [StringLength(50)]
        public string CreatedBy { get; set; }
        
        [StringLength(50)]
        public string ModifiedBy { get; set; }
        
        public bool IsActive { get; set; }
        
        public Car()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            StockQuantity = 0;
        }
    }
}
