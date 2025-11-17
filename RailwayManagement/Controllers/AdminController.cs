using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.DTOs;
using RailwayManagement.Models;
using RailwayManagement.Services;
using BCrypt.Net;
using System.Text.Json;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        private readonly JwtService _jwtService;
        
        public AdminController(RailwayDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        

        
        [HttpPost("trains")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTrain(AddTrainRequest request)
        {
            if (!TimeSpan.TryParse(request.DepartureTime, out var departureTime))
                return BadRequest("Invalid departure time format. Use HH:mm format.");
                
            if (!TimeSpan.TryParse(request.ArrivalTime, out var arrivalTime))
                return BadRequest("Invalid arrival time format. Use HH:mm format.");
            
            var train = new Train
            {
                TrainNumber = request.TrainNumber,
                TrainName = request.TrainName,
                Source = request.Source,
                Destination = request.Destination,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                TotalSeats = request.TotalSeats,
                AvailableSeats = request.TotalSeats,
                Fare = request.Fare,
                Class = request.Class
            };
            
            _context.Trains.Add(train);
            await _context.SaveChangesAsync();
            return Ok(train);
        }
        
        [HttpPut("trains/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTrain(int id, [FromBody] JsonElement request)
        {
            var existingTrain = await _context.Trains.FindAsync(id);
            if (existingTrain == null) return NotFound();
            
            try
            {
                if (request.TryGetProperty("trainName", out var trainName))
                    existingTrain.TrainName = trainName.GetString();
                    
                if (request.TryGetProperty("source", out var source))
                    existingTrain.Source = source.GetString();
                    
                if (request.TryGetProperty("destination", out var destination))
                    existingTrain.Destination = destination.GetString();
                
                if (request.TryGetProperty("departureTime", out var depTime))
                {
                    var depTimeStr = depTime.GetString();
                    if (!TimeSpan.TryParse(depTimeStr, out TimeSpan departureTime))
                        return BadRequest("Invalid departure time format. Use HH:mm format.");
                    existingTrain.DepartureTime = departureTime;
                }
                
                if (request.TryGetProperty("arrivalTime", out var arrTime))
                {
                    var arrTimeStr = arrTime.GetString();
                    if (!TimeSpan.TryParse(arrTimeStr, out TimeSpan arrivalTime))
                        return BadRequest("Invalid arrival time format. Use HH:mm format.");
                    existingTrain.ArrivalTime = arrivalTime;
                }
                
                if (request.TryGetProperty("totalSeats", out var totalSeats))
                {
                    existingTrain.TotalSeats = totalSeats.GetInt32();
                    existingTrain.AvailableSeats = totalSeats.GetInt32();
                }
                
                if (request.TryGetProperty("fare", out var fare))
                    existingTrain.Fare = fare.GetDecimal();
                
                if (request.TryGetProperty("class", out var trainClass))
                    existingTrain.Class = trainClass.GetString();
                
                await _context.SaveChangesAsync();
                return Ok(existingTrain);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }
        
        [HttpDelete("trains/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTrain(int id)
        {
            var train = await _context.Trains.FindAsync(id);
            if (train == null) return NotFound();
            
            _context.Trains.Remove(train);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Train deleted successfully" });
        }
        
        [HttpGet("bookings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Train)
                .Include(r => r.Passengers)
                .OrderByDescending(r => r.BookingDate)
                .Select(r => new
                {
                    r.PNR,
                    UserName = r.User.Name,
                    UserEmail = r.User.Email,
                    TrainName = r.Train.TrainName,
                    r.JourneyDate,
                    r.TotalFare,
                    r.Status,
                    PassengerCount = r.NumberOfPassengers
                })
                .ToListAsync();
                
            return Ok(bookings);
        }
        
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }
        
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
    
    public class AdminLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}