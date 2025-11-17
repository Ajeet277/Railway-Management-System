using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RailwayManagement.Data;
using RailwayManagement.DTOs;
using RailwayManagement.Exceptions;
using RailwayManagement.Models;
using RailwayManagement.Services;
using BCrypt.Net;
using System.Text.RegularExpressions;

namespace RailwayManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RailwayDbContext _context;
        private readonly JwtService _jwtService;
        private readonly EmailService _emailService;
        
        public AuthController(RailwayDbContext context, JwtService jwtService, EmailService emailService)
        {
            _context = context;
            _jwtService = jwtService;
            _emailService = emailService;
        }
        
        /// <summary>
        /// Registers a new user with enhanced validation and welcome email
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                // Validate email uniqueness
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    throw new ValidationException("Email already exists");
                
                // Validate phone number uniqueness
                if (await _context.Users.AnyAsync(u => u.Phone == request.Phone))
                    throw new ValidationException("Phone number already exists");
                

                

                
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = HashPassword(request.Password),
                    Address = request.Address,
                    IsEmailVerified = true // Set to true to skip verification
                };
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                // Send welcome email
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Name);
                }
                catch (Exception ex)
                {
                    // Log email error but don't fail registration
                    Console.WriteLine($"Failed to send welcome email: {ex.Message}");
                }
                
                return Ok(new { 
                    Message = "Registration successful. Please login to continue.",
                    Name = user.Name,
                    Email = user.Email
                });
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Failed to create user account", ex);
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            // Check if admin first
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == request.Email && a.IsActive);
                
            if (admin != null && VerifyPassword(request.Password, admin.Password))
            {
                var adminToken = _jwtService.GenerateToken(admin.Id, admin.Email, admin.Username, "Admin");
                return Ok(new AuthResponse
                {
                    Token = adminToken,
                    Name = admin.Username,
                    Email = admin.Email,
                    Role = "Admin"
                });
            }
            
            // Check if regular user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null || !VerifyPassword(request.Password, user.Password))
                return Unauthorized("Invalid credentials");
            
            var userToken = _jwtService.GenerateToken(user.Id, user.Email, user.Name, "User");
            
            return Ok(new AuthResponse
            {
                Token = userToken,
                Name = user.Name,
                Email = user.Email,
                Role = "User"
            });
        }
        
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }
        
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        
    }
}