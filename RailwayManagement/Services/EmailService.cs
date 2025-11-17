using System.Net;
using System.Net.Mail;

namespace RailwayManagement.Services
{
    /// <summary>
    /// Email service for sending notifications via SMTP
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        /// <summary>
        /// Sends booking confirmation email with PNR and journey details
        /// </summary>
        public async Task SendBookingConfirmationAsync(string email, string userName, string pnr, string trainName, string trainNumber, DateTime journeyDate, string fromStation, string toStation, int passengerCount, decimal totalFare)
        {
            try
            {
                var subject = $"Booking Confirmed - PNR: {pnr}";
                var body = GetBookingConfirmationTemplate(userName, pnr, trainName, trainNumber, journeyDate, fromStation, toStation, passengerCount, totalFare);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Booking confirmation email sent to {email} for PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send booking confirmation email to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends payment confirmation email
        /// </summary>
        public async Task SendPaymentConfirmationAsync(string email, string userName, string pnr, decimal amount, string paymentMethod, string transactionId)
        {
            try
            {
                var subject = $"Payment Confirmed - PNR: {pnr}";
                var body = GetPaymentConfirmationTemplate(userName, pnr, amount, paymentMethod, transactionId);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Payment confirmation email sent to {email} for PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send payment confirmation email to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends journey reminder email 24 hours before travel
        /// </summary>
        public async Task SendJourneyReminderAsync(string email, string userName, string pnr, string trainName, DateTime journeyDate, string fromStation, string toStation)
        {
            try
            {
                var subject = $"Journey Reminder - Tomorrow's Travel - PNR: {pnr}";
                var body = GetJourneyReminderTemplate(userName, pnr, trainName, journeyDate, fromStation, toStation);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Journey reminder email sent to {email} for PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send journey reminder email to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends seat upgrade notification email
        /// </summary>
        public async Task SendSeatUpgradeNotificationAsync(string email, string userName, string pnr, string oldClass, string newClass, decimal additionalAmount)
        {
            try
            {
                var subject = $"Seat Upgrade Confirmed - PNR: {pnr}";
                var body = GetSeatUpgradeTemplate(userName, pnr, oldClass, newClass, additionalAmount);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Seat upgrade notification sent to {email} for PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send seat upgrade notification to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends cancellation confirmation email with refund details
        /// </summary>
        public async Task SendCancellationConfirmationAsync(string email, string userName, string pnr, string trainName, DateTime journeyDate, decimal refundAmount, string cancellationReason)
        {
            try
            {
                var subject = $"Cancellation Confirmed - PNR: {pnr}";
                var body = GetCancellationConfirmationTemplate(userName, pnr, trainName, journeyDate, refundAmount, cancellationReason);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Cancellation email sent to {email} for PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send cancellation email to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends welcome email after user registration
        /// </summary>
        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            try
            {
                var subject = "Welcome to Railway Reservation System";
                var body = GetWelcomeEmailTemplate(userName);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Welcome email sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Sends email verification email to user
        /// </summary>
        public async Task SendEmailVerificationAsync(string email, string userName, string verificationToken)
        {
            try
            {
                var subject = "Verify Your Email - Railway Reservation System";
                var body = GetEmailVerificationTemplate(userName, verificationToken);
                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Email verification sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email verification to {email}");
                throw;
            }
        }
        
        /// <summary>
        /// Core method to send email via SMTP
        /// </summary>
        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            
            using var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]))
            {
                Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["SenderPassword"]),
                EnableSsl = bool.Parse(emailSettings["EnableSsl"])
            };
            
            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], "Railway Reservation System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
        }
        
        /// <summary>
        /// HTML template for booking confirmation email with journey details
        /// </summary>
        private string GetBookingConfirmationTemplate(string userName, string pnr, string trainName, string trainNumber, DateTime journeyDate, string fromStation, string toStation, int passengerCount, decimal totalFare)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                    <h2 style='color: #2E8B57; text-align: center;'>🎫 Booking Confirmed!</h2>
                    <p>Dear {userName},</p>
                    <p>Your train booking has been confirmed successfully. Here are your journey details:</p>
                    
                    <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #333; margin-top: 0;'>Journey Details</h3>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Train:</strong> {trainName} ({trainNumber})</p>
                        <p><strong>Journey Date:</strong> {journeyDate:dd MMM yyyy}</p>
                        <p><strong>From:</strong> {fromStation}</p>
                        <p><strong>To:</strong> {toStation}</p>
                        <p><strong>Passengers:</strong> {passengerCount}</p>
                        <p><strong>Total Fare:</strong> ₹{totalFare}</p>
                    </div>
                    
                    <p><strong>Important:</strong> Please carry a valid ID proof during your journey.</p>
                    <p>Thank you for choosing our railway service!</p>
                </div>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for payment confirmation email
        /// </summary>
        private string GetPaymentConfirmationTemplate(string userName, string pnr, decimal amount, string paymentMethod, string transactionId)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                    <h2 style='color: #4CAF50; text-align: center;'>💳 Payment Confirmed!</h2>
                    <p>Dear {userName},</p>
                    <p>Your payment has been processed successfully.</p>
                    
                    <div style='background-color: #f0f8f0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #333; margin-top: 0;'>Payment Details</h3>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Amount Paid:</strong> ₹{amount}</p>
                        <p><strong>Payment Method:</strong> {paymentMethod}</p>
                        <p><strong>Transaction ID:</strong> {transactionId}</p>
                        <p><strong>Date:</strong> {DateTime.Now:dd MMM yyyy HH:mm}</p>
                    </div>
                    
                    <p>Your booking is now confirmed. Safe travels!</p>
                </div>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for journey reminder email
        /// </summary>
        private string GetJourneyReminderTemplate(string userName, string pnr, string trainName, DateTime journeyDate, string fromStation, string toStation)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                    <h2 style='color: #FF6B35; text-align: center;'>🚂 Journey Reminder - Tomorrow!</h2>
                    <p>Dear {userName},</p>
                    <p>This is a friendly reminder that your train journey is scheduled for tomorrow.</p>
                    
                    <div style='background-color: #fff3e0; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #333; margin-top: 0;'>Journey Details</h3>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Train:</strong> {trainName}</p>
                        <p><strong>Date:</strong> {journeyDate:dd MMM yyyy}</p>
                        <p><strong>Route:</strong> {fromStation} → {toStation}</p>
                    </div>
                    
                    <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px;'>
                        <h4 style='color: #1976d2; margin-top: 0;'>Travel Checklist:</h4>
                        <ul>
                            <li>Valid ID proof (Aadhaar, PAN, Passport, etc.)</li>
                            <li>Printed or mobile ticket</li>
                            <li>Reach station 30 minutes before departure</li>
                        </ul>
                    </div>
                    
                    <p>Have a safe and pleasant journey!</p>
                </div>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for seat upgrade notification
        /// </summary>
        private string GetSeatUpgradeTemplate(string userName, string pnr, string oldClass, string newClass, decimal additionalAmount)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                    <h2 style='color: #9C27B0; text-align: center;'>⬆️ Seat Upgrade Confirmed!</h2>
                    <p>Dear {userName},</p>
                    <p>Great news! Your seat has been successfully upgraded.</p>
                    
                    <div style='background-color: #f3e5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #333; margin-top: 0;'>Upgrade Details</h3>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Upgraded From:</strong> {oldClass}</p>
                        <p><strong>Upgraded To:</strong> {newClass}</p>
                        <p><strong>Additional Amount:</strong> ₹{additionalAmount}</p>
                    </div>
                    
                    <p>Enjoy your enhanced travel experience!</p>
                </div>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for cancellation confirmation email
        /// </summary>
        private string GetCancellationConfirmationTemplate(string userName, string pnr, string trainName, DateTime journeyDate, decimal refundAmount, string cancellationReason)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd;'>
                    <h2 style='color: #f44336; text-align: center;'>❌ Booking Cancelled</h2>
                    <p>Dear {userName},</p>
                    <p>Your booking has been cancelled successfully.</p>
                    
                    <div style='background-color: #ffebee; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <h3 style='color: #333; margin-top: 0;'>Cancellation Details</h3>
                        <p><strong>PNR:</strong> {pnr}</p>
                        <p><strong>Train:</strong> {trainName}</p>
                        <p><strong>Journey Date:</strong> {journeyDate:dd MMM yyyy}</p>
                        <p><strong>Reason:</strong> {cancellationReason}</p>
                        <p><strong>Refund Amount:</strong> ₹{refundAmount}</p>
                    </div>
                    
                    <p><strong>Refund Information:</strong> The refund will be processed within 5-7 business days to your original payment method.</p>
                    <p>Thank you for using our service!</p>
                </div>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for welcome email
        /// </summary>
        private string GetWelcomeEmailTemplate(string userName)
        {
            return $@"
            <html>
            <body>
                <h2>Welcome to Railway Reservation System</h2>
                <p>Dear {userName},</p>
                <p>Welcome to our railway reservation system!</p>
                <p>You can now:</p>
                <ul>
                    <li>Search and book train tickets</li>
                    <li>View your booking history</li>
                    <li>Cancel bookings and get refunds</li>
                </ul>
                <p>Thank you for registering with us!</p>
            </body>
            </html>";
        }
        
        /// <summary>
        /// HTML template for email verification
        /// </summary>
        private string GetEmailVerificationTemplate(string userName, string verificationToken)
        {
            var verificationUrl = $"https://localhost:7000/api/auth/verify-email?token={verificationToken}";
            return $@"
            <html>
            <body>
                <h2>Email Verification Required</h2>
                <p>Dear {userName},</p>
                <p>Thank you for registering with Railway Reservation System!</p>
                <p>Please verify your email address by clicking the link below:</p>
                <p><a href='{verificationUrl}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Email Address</a></p>
                <p>Or copy and paste this link in your browser:</p>
                <p>{verificationUrl}</p>
                <p><strong>Note:</strong> This link will expire in 24 hours.</p>
                <p>If you didn't create this account, please ignore this email.</p>
            </body>
            </html>";
        }
    }
}