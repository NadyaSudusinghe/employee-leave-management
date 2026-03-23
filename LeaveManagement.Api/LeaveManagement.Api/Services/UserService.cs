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

        public async Task<bool> LinkUserToEmployee(int userId, int employeeId)
        {
            var user = await _context.Users.FindAsync(userId);
            var employee = await _context.Employees.FindAsync(employeeId);

            if(user == null ||  employee == null)
                return false;

            user.EmployeeId = employeeId;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
