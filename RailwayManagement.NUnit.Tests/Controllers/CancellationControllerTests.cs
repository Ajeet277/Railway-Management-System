using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Controllers;
using RailwayManagement.Data;
using RailwayManagement.Models;
using RailwayManagement.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;

namespace RailwayManagement.NUnit.Tests
{
    [TestFixture]
    public class CancellationControllerTests
    {
        private RailwayDbContext _context;
        private Mock<IEmailService> _mockEmailService;
        private CancellationController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RailwayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new RailwayDbContext(options);
            _mockEmailService = new Mock<IEmailService>();
            _controller = new CancellationController(_context, _mockEmailService.Object);

            // Setup user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Phone = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("password123", 12),
                Address = "Test Address"
            };

            var train = new Train
            {
                Id = 1,
                TrainNumber = "12345",
                TrainName = "Express",
                Source = "Delhi",
                Destination = "Mumbai",
                DepartureTime = TimeSpan.FromHours(10),
                ArrivalTime = TimeSpan.FromHours(20),
                TotalSeats = 100,
                AvailableSeats = 95,
                Fare = 1000,
                Class = "AC"
            };

            var reservation = new Reservation
            {
                Id = 1,
                PNR = "PNR123",
                UserId = 1,
                TrainId = 1,
                JourneyDate = DateTime.Today.AddDays(1),
                NumberOfPassengers = 2,
                TotalFare = 2000,
                Status = ReservationStatus.Confirmed,
                User = user,
                Train = train
            };

            _context.Users.Add(user);
            _context.Trains.Add(train);
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        [Test]
        public async Task CancelBooking_ValidPNR_ReturnsCancellationDetails()
        {
            // Act
            var result = await _controller.CancelBooking("PNR123", "Change of plans");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var response = okResult.Value;
            Assert.That(response, Is.Not.Null);
            
            var responseType = response.GetType();
            var messageProperty = responseType.GetProperty("Message");
            var pnrProperty = responseType.GetProperty("PNR");
            var refundAmountProperty = responseType.GetProperty("RefundAmount");
            
            Assert.That(messageProperty.GetValue(response), Is.EqualTo("Booking cancelled successfully"));
            Assert.That(pnrProperty.GetValue(response), Is.EqualTo("PNR123"));
            Assert.That(refundAmountProperty.GetValue(response), Is.EqualTo(1600m)); // 80% of 2000
        }

        [Test]
        public async Task CancelBooking_InvalidPNR_ReturnsNotFound()
        {
            // Act
            var result = await _controller.CancelBooking("INVALID", "Reason");

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CancelBooking_AlreadyCancelled_ReturnsBadRequest()
        {
            // Arrange
            var reservation = await _context.Reservations.FirstAsync();
            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.CancelBooking("PNR123", "Reason");

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetMyCancellations_ReturnsUserCancellations()
        {
            // Arrange
            var cancellation = new Cancellation
            {
                ReservationId = 1,
                Reason = "Test reason",
                RefundAmount = 1600
            };
            _context.Cancellations.Add(cancellation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetMyCancellations();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var cancellations = okResult.Value as IEnumerable<object>;
            Assert.That(cancellations, Is.Not.Null);
            Assert.That(cancellations.Count(), Is.EqualTo(1));
        }
    }
}