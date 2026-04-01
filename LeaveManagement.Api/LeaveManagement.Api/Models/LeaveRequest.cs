using LeaveManagement.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Api.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public LeaveType LeaveType { get; set; }

        public string? Reason { get; set; }

        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
