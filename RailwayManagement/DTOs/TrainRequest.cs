using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.DTOs
{
    public class AddTrainRequest
    {
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
        
        [Required]
        public string DepartureTime { get; set; }
        
        [Required]
        public string ArrivalTime { get; set; }
        
        [Required]
        [Range(1, 1000)]
        public int TotalSeats { get; set; }
        
        [Required]
        [Range(0.01, 50000)]
        public decimal Fare { get; set; }
        
        [Required]
        public string Class { get; set; }
    }
    
    public class UpdateTrainRequest
    {
        [StringLength(100)]
        public string TrainName { get; set; }
        
        public string Source { get; set; }
        public string Destination { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        
        [Range(1, 1000)]
        public int? TotalSeats { get; set; }
        
        [Range(0.01, 50000)]
        public decimal? Fare { get; set; }
        
        public string Class { get; set; }
    }
}