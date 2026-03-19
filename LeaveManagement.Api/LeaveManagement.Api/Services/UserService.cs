using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllUsers()
        {
            return await _context.Users
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                })
                .ToListAsync();
        }
    }
}
