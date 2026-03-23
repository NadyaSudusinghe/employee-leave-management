using LeaveManagement.Api.Common;
using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        //Create new Employee
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeCreateDto employeeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = await _employeeService.CreateEmployee(employeeDto);
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get all records
        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }

        //Get By Id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeId = GetEmployeeIdFromToken();

            if (role != Roles.Admin && employeeId != id)
                return Forbid();

            var employee = await _employeeService.GetEmployeeById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        //Update Employe
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto updateDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var employeeId = GetEmployeeIdFromToken();

                if (role != Roles.Admin && employeeId != id)
                    return Forbid();

                var updated = await _employeeService.UpdateEmployee(id, updateDto);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Delete Employee
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _employeeService.DeleteEmployee(id);

            if(!deleted)
                return NotFound();

            return NoContent();
        }

        private int? GetEmployeeIdFromToken()
        {
            var claim = User.FindFirst("employeeId")?.Value;

            if (string.IsNullOrEmpty(claim))
                return null;

            return int.Parse(claim);
        }
    }
}
