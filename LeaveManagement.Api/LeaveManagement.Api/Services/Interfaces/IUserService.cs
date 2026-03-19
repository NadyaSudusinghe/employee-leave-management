using LeaveManagement.Api.DTOs;

namespace LeaveManagement.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllUsers();
    }
}
