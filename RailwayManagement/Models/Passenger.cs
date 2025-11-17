using System.ComponentModel.DataAnnotations;

namespace RailwayManagement.Models
{
    public class Passenger
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
