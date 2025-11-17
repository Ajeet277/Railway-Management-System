using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class Train
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(10)]
        public string TrainNumber { get; set; }
        
        [Required]
        [StringLength(100)]
        public string TrainName { get; set; }
        
        [Required]
        public string Source { get; set; }
        
        [Required]
        public string Destination { get; set; }
        
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        
        [Required]
        public decimal Fare { get; set; }
        
        public string Class { get; set; } // AC, Sleeper, General
        
        public ICollection<Reservation> Reservations { get; set; }
    }
}