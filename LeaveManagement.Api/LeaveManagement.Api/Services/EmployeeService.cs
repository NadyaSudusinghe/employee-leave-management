using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeReadDto> CreateEmployee(EmployeeCreateDto dto)
        {
            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email == dto.Email);

            if (emailExists)
                throw new InvalidOperationException("Email is already in use.");

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateJoined = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            //auto-link with a user account, if exists
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user != null && user.EmployeeId == null)
            {
                user.EmployeeId = employee.Id;
                await _context.SaveChangesAsync();
            }

            return new EmployeeReadDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DateJoined = employee.DateJoined
            };
        }

        public async Task<IEnumerable<EmployeeReadDto>> GetAllEmployees()
        {
            return await _context.Employees
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    DateJoined = e.DateJoined
                })
                .ToListAsync();
        }

        public async Task<EmployeeReadDto?> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if(employee == null)
                return null;

            return new EmployeeReadDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DateJoined = employee.DateJoined
            };
        }

        public async Task<bool> UpdateEmployee(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return false;

            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email == dto.Email && e.Id != id);

            if (emailExists)
                throw new InvalidOperationException("Email is already in use."); ;

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if(employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
