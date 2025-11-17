using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        
        public TrainController(RailwayDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrains(string source, string destination, DateTime date)
        {
            var trains = await _context.Trains
                .Where(t => t.Source.ToLower() == source.ToLower() && 
                           t.Destination.ToLower() == destination.ToLower())
                .Select(t => new
                {
                    t.Id,
                    t.TrainNumber,
                    t.TrainName,
                    t.Source,
                    t.Destination,
                    t.DepartureTime,
                    t.ArrivalTime,
                    t.Fare,
                    t.Class,
                    t.AvailableSeats
                })
                .ToListAsync();
                
            return Ok(trains);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrain(int id)
        {
            var train = await _context.Trains
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.TrainNumber,
                    t.TrainName,
                    t.Source,
                    t.Destination,
                    t.DepartureTime,
                    t.ArrivalTime,
                    t.TotalSeats,
                    t.AvailableSeats,
                    t.Fare,
                    t.Class
                })
                .FirstOrDefaultAsync();
                
            if (train == null) return NotFound();
            return Ok(train);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllTrains()
        {
            var trains = await _context.Trains
                .OrderByDescending(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.TrainNumber,
                    t.TrainName,
                    t.Source,
                    t.Destination,
                    t.DepartureTime,
                    t.ArrivalTime,
                    t.TotalSeats,
                    t.AvailableSeats,
                    t.Fare,
                    t.Class
                })
                .ToListAsync();
            return Ok(trains);
        }
        
        [HttpGet("number/{trainNumber}")]
        public async Task<IActionResult> SearchTrainByNumber(string trainNumber)
        {
            var trains = await _context.Trains
                .Where(t => t.TrainNumber.Contains(trainNumber))
                .Select(t => new
                {
                    t.Id,
                    t.TrainNumber,
                    t.TrainName,
                    t.Source,
                    t.Destination,
                    t.DepartureTime,
                    t.ArrivalTime,
                    t.Fare,
                    t.Class,
                    t.AvailableSeats
                })
                .ToListAsync();
                
            return Ok(trains);
        }
    }
}