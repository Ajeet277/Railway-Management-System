using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.DTOs;
using RailwayManagement.Models;
using RailwayManagement.Services;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User")]
    public class PaymentController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        private readonly PaymentService _paymentService;
        private readonly EmailService _emailService;
        
        public PaymentController(RailwayDbContext context, PaymentService paymentService, EmailService emailService)
        {
            _context = context;
            _paymentService = paymentService;
            _emailService = emailService;
        }
        

        

        
        /// <summary>
        /// Process payment
        /// </summary>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessMockPayment(PaymentRequest request)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Train)
                    .Include(r => r.Passengers)
                    .FirstOrDefaultAsync(r => r.Id == request.ReservationId);
                    
                if (reservation == null) 
                    return NotFound("Reservation not found");
                
                if (reservation.Status != ReservationStatus.PendingPayment)
                    return BadRequest("Reservation is not pending payment");
                
                var paymentResponse = await _paymentService.ProcessMockPaymentAsync(request, reservation.TotalFare);
                
                // If payment successful, confirm reservation first
                if (paymentResponse.Status == "Success")
                {
                    reservation.Status = ReservationStatus.Confirmed;
                    await _context.SaveChangesAsync();
                    
                    // Send emails after successful reservation update
                    try
                    {
                        await _emailService.SendBookingConfirmationAsync(
                            reservation.User.Email,
                            reservation.User.Name,
                            reservation.PNR,
                            reservation.Train.TrainName,
                            reservation.Train.TrainNumber,
                            reservation.JourneyDate,
                            reservation.Train.Source,
                            reservation.Train.Destination,
                            reservation.Passengers.Count,
                            reservation.TotalFare
                        );
                        
                        await _emailService.SendPaymentConfirmationAsync(
                            reservation.User.Email,
                            reservation.User.Name,
                            reservation.PNR,
                            reservation.TotalFare,
                            request.PaymentMethod,
                            paymentResponse.TransactionId
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send confirmation emails: {ex.Message}");
                    }
                }
                
                // Try to save payment record (optional - don't fail if this fails)
                try
                {
                    var payment = new Payment
                    {
                        ReservationId = request.ReservationId,
                        Amount = reservation.TotalFare,
                        PaymentMethod = request.PaymentMethod,
                        TransactionId = paymentResponse.TransactionId,
                        Status = paymentResponse.Status,
                        FailureReason = paymentResponse.Status == "Failed" ? paymentResponse.Message : null,
                        CompletedAt = paymentResponse.Status == "Success" ? DateTime.Now : null,
                        CardLast4Digits = GetCardLast4Digits(request),
                        UpiId = string.IsNullOrEmpty(request.UpiId) ? null : request.UpiId,
                        BankName = GetBankName(request)
                    };
                    
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }
                catch (Exception paymentEx)
                {
                    Console.WriteLine($"Failed to save payment record: {paymentEx.Message}");
                    // Continue - payment was successful even if we can't save the payment record
                }
                
                return Ok(paymentResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Payment processing failed: {ex.Message}");
            }
        }
        

        /// <summary>
        /// Gets payment status and history for a reservation
        /// </summary>
        [HttpGet("reservation/{reservationId}")]
        public async Task<IActionResult> GetPaymentStatus(int reservationId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
                
            if (payment == null) return NotFound("Payment not found");
            
            return Ok(new
            {
                payment.TransactionId,
                payment.Status,
                payment.Amount,
                payment.PaymentDate,
                payment.PaymentMethod,
                payment.RetryAttempts,
                payment.FailureReason,
                payment.CompletedAt,
                payment.CardLast4Digits,
                payment.UpiId,
                payment.BankName
            });
        }
        
        /// <summary>
        /// Gets payment history for a user
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            
            var payments = await _context.Payments
                .Include(p => p.Reservation)
                .ThenInclude(r => r.User)
                .Include(p => p.Reservation.Train)
                .Where(p => p.Reservation.User.Email == userEmail)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new
                {
                    p.TransactionId,
                    p.Amount,
                    p.Status,
                    p.PaymentMethod,
                    p.PaymentDate,
                    p.CompletedAt,
                    PNR = p.Reservation.PNR,
                    TrainName = p.Reservation.Train.TrainName,
                    JourneyDate = p.Reservation.JourneyDate
                })
                .ToListAsync();
                
            return Ok(payments);
        }
        
        private string? GetCardLast4Digits(PaymentRequest request)
        {
            if ((request.PaymentMethod == "CreditCard" || request.PaymentMethod == "DebitCard") 
                && !string.IsNullOrEmpty(request.CardNumber) && request.CardNumber.Length >= 4)
            {
                return request.CardNumber[^4..];
            }
            return null;
        }
        
        private string? GetBankName(PaymentRequest request)
        {
            if (request.PaymentMethod == "NetBanking" && !string.IsNullOrEmpty(request.BankCode))
            {
                return request.BankCode switch
                {
                    "SBI" => "State Bank of India",
                    "HDFC" => "HDFC Bank",
                    "ICICI" => "ICICI Bank",
                    "AXIS" => "Axis Bank",
                    _ => "Other Bank"
                };
            }
            return null;
        }
    }
}