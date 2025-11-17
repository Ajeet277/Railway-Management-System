using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.DTOs;
using RailwayManagement.Exceptions;
using RailwayManagement.Models;
using RailwayManagement.Services;
using System.Security.Claims;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class ReservationController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        private readonly EmailService _emailService;
        
        public ReservationController(RailwayDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        
        /// <summary>
        /// Creates a new train reservation for authenticated user
        /// </summary>
        /// <param name="request">Reservation details including train, date and passengers</param>
        /// <returns>Reservation confirmation with PNR</returns>
        [HttpPost]
        public async Task<IActionResult> CreateReservation(ReservationRequest request)
        {
            try
            {
                // Validate user authentication
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    throw new ValidationException("User authentication required");
                
                var userId = int.Parse(userIdClaim);
                
                // Validate passenger count matches list
                if (request.NumberOfPassengers != request.Passengers?.Count)
                    throw new ValidationException("Number of passengers must match passenger list count");
                
                // Find train with null check
                var train = await _context.Trains.FindAsync(request.TrainId);
                if (train == null)
                    throw new BusinessLogicException($"Train with ID {request.TrainId} not found");
                
                // Validate train has available seats
                if (train.AvailableSeats <= 0)
                    throw new BusinessLogicException($"Train {train.TrainName} has no available seats");
                
                // Check seat availability
                if (train.AvailableSeats < request.NumberOfPassengers)
                    throw new BusinessLogicException("Not enough seats available");
                
                // Validate journey date
                if (request.JourneyDate.Date < DateTime.Today)
                    throw new ValidationException("Journey date cannot be in the past");
                
                var pnr = GeneratePNR();
                var totalFare = train.Fare * request.NumberOfPassengers;
                
                // Create passengers list
                var passengers = new List<Passenger>();
                foreach (var passengerDto in request.Passengers)
                {
                    passengers.Add(new Passenger
                    {
                        Name = passengerDto.Name,
                        Age = passengerDto.Age,
                        Gender = passengerDto.Gender
                    });
                }
                
                var reservation = new Reservation
                {
                    PNR = pnr,
                    UserId = userId,
                    TrainId = request.TrainId,
                    JourneyDate = request.JourneyDate,
                    NumberOfPassengers = request.NumberOfPassengers,
                    TotalFare = totalFare,
                    Status = ReservationStatus.PendingPayment,
                    Passengers = passengers
                };
                
                // Update available seats
                train.AvailableSeats -= request.NumberOfPassengers;
                
                // Add reservation with passengers
                _context.Reservations.Add(reservation);
                
                // Save everything in one transaction
                await _context.SaveChangesAsync();
                
                return Ok(new ReservationResponse
                {
                    ReservationId = reservation.Id,
                    PNR = pnr,
                    Status = "PendingPayment",
                    TotalFare = totalFare,
                    JourneyDate = request.JourneyDate,
                    TrainName = train.TrainName,
                    TrainNumber = train.TrainNumber
                });
            }
            catch (DbUpdateException ex)
            {
                // Log the actual exception details
                var innerException = ex.InnerException?.Message ?? "No inner exception";
                var message = $"Database error: {ex.Message}. Inner: {innerException}";
                Console.WriteLine($"RESERVATION ERROR: {message}");
                Console.WriteLine($"FULL EXCEPTION: {ex}");
                throw new DatabaseException(message, ex);
            }
            catch (FormatException ex)
            {
                throw new ValidationException("Invalid user ID format", ex);
            }
        }
        
        /// <summary>
        /// Gets all bookings for the authenticated user
        /// </summary>
        /// <returns>List of user's reservations</returns>
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    throw new ValidationException("User authentication required");
                
                var userId = int.Parse(userIdClaim);
                
                var bookings = await _context.Reservations
                    .Include(r => r.Train)
                    .Include(r => r.Passengers)
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.BookingDate)
                    .Select(r => new
                    {
                        r.Id,
                        r.PNR,
                        Status = r.Status.ToString(),
                        r.TotalFare,
                        r.JourneyDate,
                        r.BookingDate,
                        TrainName = r.Train.TrainName,
                        TrainNumber = r.Train.TrainNumber,
                        Source = r.Train.Source,
                        Destination = r.Train.Destination,
                        Passengers = r.Passengers.Select(p => new { p.Name, p.Age, p.Gender })
                    })
                    .ToListAsync();
                    
                return Ok(bookings);
            }
            catch (FormatException ex)
            {
                throw new ValidationException("Invalid user ID format", ex);
            }
        }
        
        /// <summary>
        /// Gets booking details by PNR number
        /// </summary>
        /// <param name="pnr">PNR number to search</param>
        /// <returns>Booking details if found</returns>
        [HttpGet("pnr/{pnr}")]
        public async Task<IActionResult> GetBookingByPNR(string pnr)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pnr))
                    throw new ValidationException("PNR number is required");
                
                var booking = await _context.Reservations
                    .Include(r => r.Train)
                    .Include(r => r.Passengers)
                    .FirstOrDefaultAsync(r => r.PNR == pnr);
                    
                if (booking == null)
                    throw new BusinessLogicException("Booking not found");
                
                return Ok(new
                {
                    booking.PNR,
                    Status = booking.Status.ToString(),
                    booking.TotalFare,
                    booking.JourneyDate,
                    booking.BookingDate,
                    TrainName = booking.Train.TrainName,
                    TrainNumber = booking.Train.TrainNumber,
                    Source = booking.Train.Source,
                    Destination = booking.Train.Destination,
                    Passengers = booking.Passengers.Select(p => new { p.Name, p.Age, p.Gender })
                });
            }
            catch (ArgumentException ex)
            {
                throw new ValidationException("Invalid PNR format", ex);
            }
        }
        
        /// <summary>
        /// Generates unique PNR number based on current timestamp
        /// </summary>
        /// <returns>10-digit PNR string</returns>
        private string GeneratePNR()
        {
            return DateTime.Now.ToString("yyMMddHHmm");
        }
    }
}