using LeaveManagement.Api.Common;
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

        public async Task<LeaveRequestReadDto> CreateLeaveRequest(LeaveRequestCreateDto dto, int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
                throw new InvalidOperationException("Employee does not Exist.");

            var leaveRequest = new LeaveRequest
            {
                StartDate = dto.StartDate.ToUniversalTime(),
                EndDate = dto.EndDate.ToUniversalTime(),
                Reason = dto.Reason,
                EmployeeId = employeeId,
                Status = LeaveRequestStatus.Pending,
                LeaveType = dto.LeaveType!.Value
            };

            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();

            return new LeaveRequestReadDto
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                Reason = leaveRequest.Reason ?? string.Empty,
                Status = leaveRequest.Status,
                LeaveType = leaveRequest.LeaveType,
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
                Reason = lr.Reason ?? string.Empty,
                Status = lr.Status,
                LeaveType = lr.LeaveType,
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
                Reason = leaveRequest.Reason ?? string.Empty,
                Status = leaveRequest.Status,
                LeaveType = leaveRequest.LeaveType,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = leaveRequest.Employee.FirstName + " " + leaveRequest.Employee.LastName
            };
        }

        public async Task<bool> UpdateLeaveRequest(int id, LeaveRequestCreateDto dto, int employeeId)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null)
                return false;

            //check if employee exists as well.
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);

            if (!employeeExists)
                throw new InvalidOperationException("Employee does not exist.");

            if (employeeId != leaveRequest.EmployeeId)
                throw new UnauthorizedAccessException("You cannot modify this leave request");

            leaveRequest.StartDate = dto.StartDate.ToUniversalTime();
            leaveRequest.EndDate = dto.EndDate.ToUniversalTime();
            leaveRequest.Reason = dto.Reason;
            leaveRequest.LeaveType = dto.LeaveType!.Value;

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
                Reason = lr.Reason ?? string.Empty,
                Status = lr.Status,
                LeaveType = lr.LeaveType,
                EmployeeId = lr.EmployeeId,
                EmployeeName = lr.Employee.FirstName + " " + lr.Employee.LastName
            }).ToListAsync();
        }

        public async Task<bool> UpdateLeaveRequestStatus(int id, LeaveRequestStatus status)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);

            if (leaveRequest == null) 
                return false;

            //validate status
            //var validStatuses = new[]
            //{
            //    LeaveRequestStatus.Pending,
            //    LeaveRequestStatus.Approved,
            //    LeaveRequestStatus.Rejected,
            //};

            //if (!validStatuses.Contains(status))
            //    throw new ArgumentException("Invalid status value");

            if (leaveRequest.Status != LeaveRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be updated");

            leaveRequest.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
