using RailwayManagement.DTOs;
using RailwayManagement.Models;

namespace RailwayManagement.Services
{
    public class PaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly Random _random = new();
        
        public PaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        

        
        /// <summary>
        /// Processes mock payment with different scenarios
        /// </summary>
        public async Task<PaymentResponse> ProcessMockPaymentAsync(PaymentRequest request, decimal amount)
        {
            await Task.Delay(_random.Next(500, 2000));
            
            var transactionId = GenerateTransactionId(request.PaymentMethod);
            var response = new PaymentResponse
            {
                TransactionId = transactionId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                PaymentMethod = request.PaymentMethod
            };
            
            var scenario = GetPaymentScenario(request.PaymentMethod);
            
            switch (scenario)
            {
                case "Success":
                    response.Status = "Success";
                    response.Message = "Payment processed successfully";
                    break;
                    
                case "Failed":
                    response.Status = "Failed";
                    response.Message = GetFailureReason(request.PaymentMethod);
                    break;
                    
                case "Pending":
                    response.Status = "Pending";
                    response.Message = "Payment is being processed";
                    break;
            }
            
            return response;
        }
        
        /// <summary>
        /// Retries failed payment
        /// </summary>
        public async Task<PaymentResponse> RetryPaymentAsync(PaymentRequest request, decimal amount, int retryAttempt)
        {
            return await ProcessMockPaymentAsync(request, amount);
        }
        
        /// <summary>
        /// Generates transaction ID based on payment method
        /// </summary>
        private string GenerateTransactionId(string paymentMethod)
        {
            var prefix = paymentMethod switch
            {
                "CreditCard" => "CC",
                "DebitCard" => "DC",
                "UPI" => "UPI",
                "NetBanking" => "NB",
                _ => "TXN"
            };
            
            return $"{prefix}{DateTime.Now:yyyyMMdd}{_random.Next(100000, 999999)}";
        }
        
        /// <summary>
        /// Determines payment scenario based on method and random factors
        /// </summary>
        private string GetPaymentScenario(string paymentMethod)
        {
            var randomValue = _random.NextDouble();
            
            return paymentMethod switch
            {
                "CreditCard" => randomValue < 0.85 ? "Success" : randomValue < 0.95 ? "Failed" : "Pending",
                "DebitCard" => randomValue < 0.80 ? "Success" : randomValue < 0.92 ? "Failed" : "Pending",
                "UPI" => randomValue < 0.90 ? "Success" : randomValue < 0.97 ? "Failed" : "Pending",
                "NetBanking" => randomValue < 0.75 ? "Success" : randomValue < 0.90 ? "Failed" : "Pending",
                _ => "Success"
            };
        }
        
        /// <summary>
        /// Gets appropriate failure reason based on payment method
        /// </summary>
        private string GetFailureReason(string paymentMethod)
        {
            var reasons = paymentMethod switch
            {
                "CreditCard" => new[] { "Insufficient credit limit", "Card expired", "Invalid CVV", "Card blocked" },
                "DebitCard" => new[] { "Insufficient balance", "Daily limit exceeded", "Card expired", "PIN incorrect" },
                "UPI" => new[] { "UPI PIN incorrect", "Transaction declined by bank", "UPI service unavailable", "Daily limit exceeded" },
                "NetBanking" => new[] { "Session timeout", "Invalid credentials", "Bank server unavailable", "Transaction limit exceeded" },
                _ => new[] { "Payment failed" }
            };
            
            return reasons[_random.Next(reasons.Length)];
        }
    }
}