using LeaveManagement.Api.Common;
using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs.Auth;
using LeaveManagement.Api.Models;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Services
{
    public class AuthService: IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> Register(RegisterDto dto)
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new InvalidOperationException("User already exists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            //auto-link to an employee if an employee exists
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == dto.Email);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role ?? Roles.User,
                EmployeeId = employee?.Id
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _jwtService.GenerateToken(user);
        }

        public async Task<string> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid credentials.");

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!valid)
                throw new InvalidOperationException("Invalid credentials.");

            return _jwtService.GenerateToken(user);
        }
    }
}
