using LeaveManagement.Api.Common;
using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestStatusUpdateDto
    {
        [Required]
        public LeaveRequestStatus Status { get; set; }
    }
}
