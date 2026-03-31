using LeaveManagement.Api.Common;
using LeaveManagement.Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Api.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestReadDto> CreateLeaveRequest(LeaveRequestCreateDto dto, int employeeId);
        Task<IEnumerable<LeaveRequestReadDto>> GetAllLeaveRequests();
        Task<LeaveRequestReadDto?> GetLeaveRequestById(int id);
        Task<bool> UpdateLeaveRequest(int id, LeaveRequestCreateDto dto, int employeeId);
        Task<bool> DeleteLeaveRequest(int id);
        Task<IEnumerable<LeaveRequestReadDto>> GetLeaveRequestsByEmployeeId(int id);
        Task<bool> UpdateLeaveRequestStatus(int id, LeaveRequestStatus status);
    }
}
