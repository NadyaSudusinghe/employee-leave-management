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

            await ValidateLeaveBalance(employeeId, dto);

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

        public async Task<IEnumerable<LeaveRequestReadDto>> GetAllLeaveRequests(LeaveRequestStatus? status, PaginationParams pagination)
        {
            var query = _context.LeaveRequests.Include(lr => lr.Employee).AsQueryable();

            if (status.HasValue)
                query = query.Where(lr => lr.Status == status.Value);

            return await query
                .OrderByDescending(lr => lr.StartDate)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(lr => new LeaveRequestReadDto
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

            await ValidateLeaveBalance(employeeId, dto, id);

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

        public async Task<IEnumerable<LeaveRequestReadDto>> GetLeaveRequestsByEmployeeId(int employeeId, PaginationParams pagination)
        {
            var employeeExists = await _context.Employees.AnyAsync(e => e.Id == employeeId);

            if (!employeeExists)
                throw new InvalidOperationException("Employee does not Exist.");

            return await _context.LeaveRequests.Where(lr => lr.EmployeeId == employeeId).Include(lr => lr.Employee)
                .OrderByDescending(lr => lr.StartDate)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(lr => new LeaveRequestReadDto
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

            var pendingLeaveRequest = new LeaveRequestCreateDto
            {
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                LeaveType = leaveRequest.LeaveType,
            };

            await ValidateLeaveBalance(leaveRequest.EmployeeId, pendingLeaveRequest, id);

            leaveRequest.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LeaveBalanceDto> GetLeaveBalance(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);

            if (employee == null)
                throw new InvalidOperationException("Employee does not exist.");

            var approvedRequests = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId && lr.Status == LeaveRequestStatus.Approved).ToListAsync();

            int annualUsed = approvedRequests.Where(lr => lr.LeaveType == LeaveType.Annual)
                .Sum(lr => (lr.EndDate.Date - lr.StartDate.Date).Days + 1);

            int casualUsed = approvedRequests.Where(lr => lr.LeaveType == LeaveType.Casual)
                .Sum(lr => (lr.EndDate.Date - lr.StartDate.Date).Days + 1);

            return new LeaveBalanceDto
            {
                EmployeeId = employeeId,
                EmployeeName = employee.FirstName + " " + employee.LastName,
                AnnualLeaveUsed = annualUsed,
                AnnualLeaveRemaining = LeaveLimit.AnnualLeaveLimit - annualUsed,
                CasualLeaveUsed = casualUsed,
                CasualLeaveRemaining = LeaveLimit.CasualLeaveLimit - casualUsed
            };
        }

        private async Task ValidateLeaveBalance(int employeeId, LeaveRequestCreateDto dto, int? leaveRequestId = null)
        {
            if (dto.LeaveType == LeaveType.SickLeave)
                return;

            int requestedDays = (dto.EndDate.Date -  dto.StartDate.Date).Days + 1;

            var approvedRequests = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == employeeId
                && lr.Status == LeaveRequestStatus.Approved
                && lr.LeaveType == dto.LeaveType)
                .ToListAsync();

            //In case of updating an existing leave request
            if(leaveRequestId.HasValue)
            {
                approvedRequests = approvedRequests
                    .Where(lr => lr.Id != leaveRequestId.Value).ToList();
            }

            int alreadyUsedLeaves = approvedRequests.Sum(lr => (lr.EndDate.Date - lr.StartDate.Date).Days + 1);

            switch (dto.LeaveType)
            {
                case LeaveType.Annual:
                    if (alreadyUsedLeaves + requestedDays > LeaveLimit.AnnualLeaveLimit)
                        throw new InvalidOperationException("Insufficient annual leave balance. " + $"Requested: {requestedDays} day(s), Remaining: {LeaveLimit.AnnualLeaveLimit - alreadyUsedLeaves} day(s).");
                    break;
                case LeaveType.Casual:
                    if (alreadyUsedLeaves + requestedDays > LeaveLimit.CasualLeaveLimit)
                        throw new InvalidOperationException("Insufficient casual leave balance. " + $"Requested: {requestedDays} day(s), Remaining: {LeaveLimit.CasualLeaveLimit - alreadyUsedLeaves} day(s).");
                    break;
            }
        }
    }
}
