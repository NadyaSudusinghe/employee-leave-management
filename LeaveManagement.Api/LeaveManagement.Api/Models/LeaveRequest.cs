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

        public string Reason { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
