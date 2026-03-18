using LeaveManagement.Api.Models;

namespace LeaveManagement.Api.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
