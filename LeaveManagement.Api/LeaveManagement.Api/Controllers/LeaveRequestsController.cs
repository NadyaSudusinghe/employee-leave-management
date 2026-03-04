using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequestReadDto>> CreateLeaveRequest(LeaveRequestCreateDto dto)
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);

            if (employee == null)
                return BadRequest("Employee does not Exist.");

            var leaveRequest = new LeaveRequest
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                EmployeeId = dto.EmployeeId,
                Status = "Pending"
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return Ok(new LeaveRequestReadDto
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetAllLeaveRequests()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.Employee).ToListAsync();

            var result = leaveRequests.Select(lr => new LeaveRequestReadDto
            {
                Id = lr.Id,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestReadDto>> GetLeaveRequestById(int id)
        {
            var leaveRequest = await _context.LeaveRequests.Include(lr => lr.Employee).FirstOrDefaultAsync(lr => lr.Id == id); ;

            if (leaveRequest == null)
                return NotFound();

            return Ok(new LeaveRequestReadDto
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = leaveRequest.Employee.FirstName + " " + leaveRequest.Employee.LastName
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestCreateDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
                return NotFound();

            leaveRequest.StartDate = dto.StartDate.ToUniversalTime();
            leaveRequest.EndDate = dto.EndDate.ToUniversalTime();
            leaveRequest.Reason = dto.Reason;
            leaveRequest.EmployeeId = dto.EmployeeId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
                return NotFound();

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
