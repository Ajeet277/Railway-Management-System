using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.Models;
using RailwayManagement.Services;
using System.Security.Claims;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class CancellationController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        private readonly EmailService _emailService;
        
        public CancellationController(RailwayDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        
        [HttpPost("{pnr}")]
        public async Task<IActionResult> CancelBooking(string pnr, [FromBody] string reason)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("User not authenticated");
                
                var userId = int.Parse(userIdClaim);
                
                if (string.IsNullOrEmpty(reason))
                    reason = "User requested cancellation";
            
            var reservation = await _context.Reservations
                .Include(r => r.Train)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.PNR == pnr && r.UserId == userId);
                
            if (reservation == null) return NotFound("Booking not found");
            if (reservation.Status == ReservationStatus.Cancelled) return BadRequest("Booking already cancelled");
            
            // Calculate refund amount (80% of total fare as cancellation charges)
            var refundAmount = reservation.TotalFare * 0.8m;
            
            var cancellation = new Cancellation
            {
                ReservationId = reservation.Id,
                Reason = reason,
                RefundAmount = refundAmount
            };
            
            reservation.Status = ReservationStatus.Cancelled;
            reservation.Train.AvailableSeats += reservation.NumberOfPassengers;
            
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();
            
            await _emailService.SendCancellationConfirmationAsync(
                reservation.User.Email,
                reservation.User.Name,
                pnr,
                reservation.Train.TrainName,
                reservation.JourneyDate,
                refundAmount,
                reason
            );
            
            return Ok(new
            {
                Message = "Booking cancelled successfully",
                PNR = pnr,
                RefundAmount = refundAmount,
                Status = "Cancelled"
            });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Cancellation failed: {ex.Message}");
            }
        }
        
        [HttpGet("my-cancellations")]
        public async Task<IActionResult> GetMyCancellations()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var cancellations = await _context.Cancellations
                .Include(c => c.Reservation)
                .ThenInclude(r => r.Train)
                .Where(c => c.Reservation.UserId == userId)
                .Select(c => new
                {
                    PNR = c.Reservation.PNR,
                    TrainName = c.Reservation.Train.TrainName,
                    c.RefundAmount,
                    c.RefundStatus,
                    c.CancellationDate,
                    c.Reason
                })
                .ToListAsync();
                
            return Ok(cancellations);
        }
    }
}