using LeaveManagement.Api.Common;

namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestReadDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public LeaveRequestStatus Status {  get; set; } = LeaveRequestStatus.Pending;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
    }
}
