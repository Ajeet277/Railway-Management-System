using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RailwayManagement.Controllers;
using RailwayManagement.Data;
using RailwayManagement.DTOs;
using RailwayManagement.Services;
using RailwayManagement.NUnit.Tests.Helpers;
using System.Security.Claims;

namespace RailwayManagement.NUnit.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private RailwayDbContext _context;
        private JwtService _jwtService;
        private EmailService _emailService;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("test_key_1234567890123456789012345678901234567890");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            
            _jwtService = new JwtService(mockConfig.Object);
            
            var mockEmailConfig = new Mock<IConfigurationSection>();
            mockEmailConfig.Setup(x => x["SmtpServer"]).Returns("smtp.test.com");
            mockEmailConfig.Setup(x => x["SmtpPort"]).Returns("587");
            mockEmailConfig.Setup(x => x["SenderEmail"]).Returns("test@test.com");
            mockEmailConfig.Setup(x => x["SenderPassword"]).Returns("password");
            mockEmailConfig.Setup(x => x["EnableSsl"]).Returns("true");
            mockConfig.Setup(x => x.GetSection("EmailSettings")).Returns(mockEmailConfig.Object);
            
            var mockLogger = new Mock<ILogger<EmailService>>();
            _emailService = new EmailService(mockConfig.Object, mockLogger.Object);
            
            _controller = new AuthController(_context, _jwtService, _emailService);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Register_ValidUser_ShouldReturnOk()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "New User",
                Email = "newuser@example.com",
                Password = "Password123!",
                Phone = "9876543210",
                Address = "Test Address"
            };

            // Act
            var result = await _controller.Register(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task Register_DuplicateEmail_ShouldThrowValidationException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Name = "Test User",
                Email = "test@example.com", // This email already exists in test data
                Password = "Password123!",
                Phone = "9876543210",
                Address = "Test Address"
            };

            // Act & Assert
            Assert.ThrowsAsync<RailwayManagement.Exceptions.ValidationException>(async () => await _controller.Register(request));
        }

        [Test]
        public async Task Login_ValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test123!"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task Login_InvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Test]
        public async Task Login_NonExistentUser_ShouldReturnUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}