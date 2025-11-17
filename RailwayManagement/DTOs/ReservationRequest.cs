using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.DTOs
{
    public class ReservationRequest
    {
        [Required]
        public int TrainId { get; set; }
        
        [Required]
        public DateTime JourneyDate { get; set; }
        
        [Required]
        [Range(1, 6, ErrorMessage = "Maximum 6 passengers allowed per booking")]
        public int NumberOfPassengers { get; set; }
        
        [Required]
        public List<PassengerDto> Passengers { get; set; }
    }
    
    public class PassengerDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Range(1, 120)]
        public int Age { get; set; }
        
        [Required]
        public string Gender { get; set; }
    }
    
    public class ReservationResponse
    {
        public int ReservationId { get; set; }
        public string PNR { get; set; }
        public string Status { get; set; } // "PendingPayment", "Confirmed", "Cancelled"
        public decimal TotalFare { get; set; }
        public DateTime JourneyDate { get; set; }
        public string TrainName { get; set; }
        public string TrainNumber { get; set; }
    }
}