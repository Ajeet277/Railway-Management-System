using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Controllers;
using RailwayManagement.Data;
using RailwayManagement.Models;

namespace RailwayManagement.NUnit.Tests
{
    [TestFixture]
    public class TrainControllerTests
    {
        private RailwayDbContext _context;
        private TrainController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RailwayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new RailwayDbContext(options);
            _controller = new TrainController(_context);

            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedTestData()
        {
            var trains = new List<Train>
            {
                new Train
                {
                    Id = 1,
                    TrainNumber = "12345",
                    TrainName = "Delhi Express",
                    Source = "Delhi",
                    Destination = "Mumbai",
                    DepartureTime = TimeSpan.FromHours(10),
                    ArrivalTime = TimeSpan.FromHours(20),
                    TotalSeats = 100,
                    AvailableSeats = 95,
                    Fare = 1000,
                    Class = "AC"
                },
                new Train
                {
                    Id = 2,
                    TrainNumber = "67890",
                    TrainName = "Mumbai Express",
                    Source = "Mumbai",
                    Destination = "Chennai",
                    DepartureTime = TimeSpan.FromHours(8),
                    ArrivalTime = TimeSpan.FromHours(18),
                    TotalSeats = 120,
                    AvailableSeats = 110,
                    Fare = 800,
                    Class = "Sleeper"
                }
            };

            _context.Trains.AddRange(trains);
            _context.SaveChanges();
        }

        [Test]
        public async Task SearchTrains_ValidRoute_ReturnsMatchingTrains()
        {
            // Act
            var result = await _controller.SearchTrains("Delhi", "Mumbai", DateTime.Today);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var trains = okResult.Value as IEnumerable<object>;
            Assert.That(trains, Is.Not.Null);
            Assert.That(trains.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task SearchTrains_NoMatchingRoute_ReturnsEmptyList()
        {
            // Act
            var result = await _controller.SearchTrains("Delhi", "Kolkata", DateTime.Today);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var trains = okResult.Value as IEnumerable<object>;
            Assert.That(trains, Is.Not.Null);
            Assert.That(trains.Count(), Is.EqualTo(0));
        }

        [TestCase("delhi", "mumbai")]
        [TestCase("DELHI", "MUMBAI")]
        [TestCase("Delhi", "Mumbai")]
        public async Task SearchTrains_CaseInsensitive_ReturnsMatchingTrains(string source, string destination)
        {
            // Act
            var result = await _controller.SearchTrains(source, destination, DateTime.Today);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var trains = okResult.Value as IEnumerable<object>;
            Assert.That(trains, Is.Not.Null);
            Assert.That(trains.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetTrain_ValidId_ReturnsTrain()
        {
            // Act
            var result = await _controller.GetTrain(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public async Task GetTrain_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetTrain(999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetAllTrains_ReturnsAllTrains()
        {
            // Act
            var result = await _controller.GetAllTrains();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var trains = okResult.Value as IEnumerable<object>;
            Assert.That(trains, Is.Not.Null);
            Assert.That(trains.Count(), Is.EqualTo(2));
        }
    }
}