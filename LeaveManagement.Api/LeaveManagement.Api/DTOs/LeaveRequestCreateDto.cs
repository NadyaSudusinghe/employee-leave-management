using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestCreateDto
    {
        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [MaxLength(200, ErrorMessage = "Reason cannot exceed 200 characters.")]
        public string Reason { get; set; } = string.Empty;

        //[Required(ErrorMessage = "EmployeeId is required.")]
        //public int EmployeeId { get; set; }
    }
}
