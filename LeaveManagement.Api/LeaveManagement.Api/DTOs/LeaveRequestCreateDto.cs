using LeaveManagement.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestCreateDto
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [MaxLength(200, ErrorMessage = "Reason cannot exceed 200 characters.")]
        public string? Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Leave Type is required")]
        public LeaveType? LeaveType { get; set; }

        //[Required(ErrorMessage = "EmployeeId is required.")]
        //public int EmployeeId { get; set; }
    }
}
