using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Create new Employee
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email == employeeDto.Email);

            if(emailExists)
                return BadRequest("Email is already in use.");

            var employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                DateJoined = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var readDto = new EmployeeReadDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DateJoined = employee.DateJoined
            };

            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, readDto);
        }

        //Get all records
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeReadDto>>> GetAllEmployees()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeeReadDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    DateJoined = e.DateJoined
                }).ToListAsync();
            return Ok(employees);
        }

        //Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeReadDto>> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            var readDto = new EmployeeReadDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                DateJoined = employee.DateJoined
            };

            return Ok(readDto);
        }

        //Update Employe
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto updateDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingEmployee = await _context.Employees.FindAsync(id);

            if (existingEmployee == null)
                return NotFound();

            var emailExists = await _context.Employees
                .AnyAsync(e => e.Email == updateDto.Email && e.Id != id);

            if (emailExists)
                return BadRequest("Email is already in use.");

            existingEmployee.FirstName = updateDto.FirstName;
            existingEmployee.LastName = updateDto.LastName;
            existingEmployee.Email = updateDto.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Delete Employee
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if(employee == null)
                return NotFound();

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
