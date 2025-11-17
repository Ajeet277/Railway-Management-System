using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.Models;
using BCrypt.Net;

namespace RailwayManagement.NUnit.Tests.Helpers
{
    public static class TestDbContextFactory
    {
        public static RailwayDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<RailwayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new RailwayDbContext(options);
            SeedTestData(context);
            return context;
        }

        private static void SeedTestData(RailwayDbContext context)
        {
            var user = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                Password = HashPassword("Test123!"),
                Phone = "1234567890",
                Address = "Test Address",
                IsEmailVerified = true
            };

            var train = new Train
            {
                Id = 1,
                TrainName = "Test Express",
                TrainNumber = "12345",
                Source = "Delhi",
                Destination = "Mumbai",
                DepartureTime = TimeSpan.FromHours(10),
                ArrivalTime = TimeSpan.FromHours(20),
                TotalSeats = 100,
                AvailableSeats = 50,
                Fare = 1500,
                Class = "AC"
            };

            context.Users.Add(user);
            context.Trains.Add(train);
            context.SaveChanges();
        }
        
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }
    }
}