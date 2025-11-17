using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RailwayManagement.DTOs;
using RailwayManagement.Services;

namespace RailwayManagement.NUnit.Tests.Services
{
    [TestFixture]
    public class PaymentServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private PaymentService _paymentService;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["Razorpay:KeyId"]).Returns("test_key");
            _mockConfiguration.Setup(c => c["Razorpay:KeySecret"]).Returns("test_secret");
            
            _paymentService = new PaymentService(_mockConfiguration.Object);
        }

        [Test]
        [TestCase("CreditCard")]
        [TestCase("DebitCard")]
        [TestCase("UPI")]
        [TestCase("NetBanking")]
        public async Task ProcessMockPaymentAsync_ShouldReturnValidResponse(string paymentMethod)
        {
            // Arrange
            var request = new PaymentRequest
            {
                ReservationId = 1,
                PaymentMethod = paymentMethod,
                CardNumber = "4111111111111111",
                CardHolderName = "Test User"
            };
            decimal amount = 1500;

            // Act
            var result = await _paymentService.ProcessMockPaymentAsync(request, amount);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(amount);
            result.PaymentMethod.Should().Be(paymentMethod);
            result.TransactionId.Should().NotBeNullOrEmpty();
            result.Status.Should().BeOneOf("Success", "Failed", "Pending");
        }

        [Test]
        public async Task ProcessMockPaymentAsync_CreditCard_ShouldGenerateCorrectTransactionId()
        {
            // Arrange
            var request = new PaymentRequest
            {
                ReservationId = 1,
                PaymentMethod = "CreditCard"
            };

            // Act
            var result = await _paymentService.ProcessMockPaymentAsync(request, 1000);

            // Assert
            result.TransactionId.Should().StartWith("CC");
        }

        [Test]
        public async Task RetryPaymentAsync_ShouldCallProcessMockPayment()
        {
            // Arrange
            var request = new PaymentRequest
            {
                ReservationId = 1,
                PaymentMethod = "UPI"
            };

            // Act
            var result = await _paymentService.RetryPaymentAsync(request, 1000, 1);

            // Assert
            result.Should().NotBeNull();
            result.PaymentMethod.Should().Be("UPI");
        }
    }
}