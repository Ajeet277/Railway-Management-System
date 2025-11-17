namespace RailwayManagement.Services
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string email, string name, string role = "User");
    }
}