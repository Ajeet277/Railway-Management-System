using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.Models;

namespace RailwayManagement.Services
{
    public class PaymentTimeoutService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PaymentTimeoutService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute
        private readonly TimeSpan _paymentTimeout = TimeSpan.FromMinutes(5); // 5 minute timeout

        public PaymentTimeoutService(IServiceProvider serviceProvider, ILogger<PaymentTimeoutService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCancelExpiredReservations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking expired reservations");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckAndCancelExpiredReservations()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RailwayDbContext>();

            var cutoffTime = DateTime.Now.Subtract(_paymentTimeout);

            var expiredReservations = await context.Reservations
                .Include(r => r.Train)
                .Where(r => r.Status == ReservationStatus.PendingPayment && 
                           r.BookingDate < cutoffTime)
                .ToListAsync();

            foreach (var reservation in expiredReservations)
            {
                // Cancel the reservation
                reservation.Status = ReservationStatus.Cancelled;
                
                // Release the seats back to available pool
                reservation.Train.AvailableSeats += reservation.NumberOfPassengers;
                
                _logger.LogInformation($"Auto-cancelled expired reservation PNR: {reservation.PNR}");
            }

            if (expiredReservations.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation($"Auto-cancelled {expiredReservations.Count} expired reservations");
            }
        }
    }
}