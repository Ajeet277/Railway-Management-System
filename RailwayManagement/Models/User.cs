using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(15)]
        public string Phone { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsEmailVerified { get; set; } = true;
        
        public string? EmailVerificationToken { get; set; }
        
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        
        public ICollection<Reservation> Reservations { get; set; }
    }
}