using LeaveManagement.Api.Common;

namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestQueryParams : PaginationParams 
    {
        public LeaveRequestStatus? Status { get; set; }
        public LeaveType? LeaveType { get; set; }
        public int? EmployeeId { get; set; }
    }
}
