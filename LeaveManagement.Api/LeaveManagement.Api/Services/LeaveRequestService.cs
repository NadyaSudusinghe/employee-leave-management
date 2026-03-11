using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Api.Services
{
    public class LeaveRequestService: ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveRequestReadDto> CreateLeaveRequest(LeaveRequestCreateDto dto)
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);

            if (employee == null)
                throw new InvalidOperationException("Employee does not Exist.");

            var leaveRequest = new LeaveRequest
            {
                StartDate = dto.StartDate.ToUniversalTime(),
                EndDate = dto.EndDate.ToUniversalTime(),
                Reason = dto.Reason,
                EmployeeId = dto.EmployeeId,
                Status = "Pending"
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return new LeaveRequestReadDto
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName
            };

        }

        public async Task<IEnumerable<LeaveRequestReadDto>> GetAllLeaveRequests()
        {

            return await _context.LeaveRequests.Include(lr=> lr.Employee).Select(lr => new LeaveRequestReadDto
            {
                Id = lr.Id,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName
            }).ToListAsync();
        }

        public async Task<LeaveRequestReadDto?> GetLeaveRequestById(int id)
        {
            var leaveRequest = await _context.LeaveRequests.Include(lr => lr.Employee).FirstOrDefaultAsync(lr => lr.Id == id); 

            if (leaveRequest == null)
                return null;

            return new LeaveRequestReadDto
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = leaveRequest.Employee.FirstName + " " + leaveRequest.Employee.LastName
            };
        }

        public async Task<bool> UpdateLeaveRequest(int id, LeaveRequestCreateDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
                return false;

            //check if employee exists as well.
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == dto.EmployeeId);

            if (!employeeExists)
                throw new InvalidOperationException("Employee does not exist.");

            leaveRequest.StartDate = dto.StartDate.ToUniversalTime();
            leaveRequest.EndDate = dto.EndDate.ToUniversalTime();
            leaveRequest.Reason = dto.Reason;
            leaveRequest.EmployeeId = dto.EmployeeId;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
                return false;

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LeaveRequestReadDto>> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);

            if (!employeeExists)
                throw new InvalidOperationException("Employee does not Exist.");

            return await _context.LeaveRequests.Where(lr => lr.EmployeeId == employeeId).Include(lr => lr.Employee).Select(lr => new LeaveRequestReadDto
            {
                Id = lr.Id,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName
            }).ToListAsync();
        }
    }
}
