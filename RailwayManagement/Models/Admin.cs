using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; } // Hashed password
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}