using LeaveManagement.Api.DTOs.Auth;

namespace LeaveManagement.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(RegisterDto dto);
        Task<string> Login(LoginDto dto);
    }
}
