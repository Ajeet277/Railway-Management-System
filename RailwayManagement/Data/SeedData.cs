using RailwayManagement.Models;
using BCrypt.Net;

namespace RailwayManagement.Data
{
    public static class SeedData
    {
        public static void Initialize(RailwayDbContext context)
        {
            // Seed admin if not exists
            if (!context.Admins.Any())
            {
                var admin = new Admin
                {
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123!", 12),
                    Email = "admin@railway.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                
                context.Admins.Add(admin);
                context.SaveChanges();
            }
            
            if (context.Trains.Any()) return;
            
            var trains = new[]
            {
                new Train
                {
                    TrainNumber = "12345",
                    TrainName = "Rajdhani Express",
                    Source = "Delhi",
                    Destination = "Mumbai",
                    DepartureTime = new TimeSpan(16, 30, 0),
                    ArrivalTime = new TimeSpan(8, 30, 0),
                    TotalSeats = 200,
                    AvailableSeats = 200,
                    Fare = 2500,
                    Class = "AC"
                },
                new Train
                {
                    TrainNumber = "12346",
                    TrainName = "Shatabdi Express",
                    Source = "Delhi",
                    Destination = "Chandigarh",
                    DepartureTime = new TimeSpan(7, 20, 0),
                    ArrivalTime = new TimeSpan(10, 45, 0),
                    TotalSeats = 150,
                    AvailableSeats = 150,
                    Fare = 800,
                    Class = "AC"
                },
                new Train
                {
                    TrainNumber = "12347",
                    TrainName = "Duronto Express",
                    Source = "Mumbai",
                    Destination = "Kolkata",
                    DepartureTime = new TimeSpan(22, 15, 0),
                    ArrivalTime = new TimeSpan(18, 30, 0),
                    TotalSeats = 300,
                    AvailableSeats = 300,
                    Fare = 1800,
                    Class = "Sleeper"
                }
            };
            
            context.Trains.AddRange(trains);
            context.SaveChanges();
        }
        

    }
}