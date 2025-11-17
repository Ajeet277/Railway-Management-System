namespace RailwayManagement.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string email, string userName, string pnr, string trainName, string trainNumber, DateTime journeyDate, string fromStation, string toStation, int passengerCount, decimal totalFare);
        Task SendPaymentConfirmationAsync(string email, string userName, string pnr, decimal amount, string paymentMethod, string transactionId);
        Task SendCancellationConfirmationAsync(string email, string userName, string pnr, string trainName, DateTime journeyDate, decimal refundAmount, string cancellationReason);
        Task SendWelcomeEmailAsync(string email, string userName);
    }
}