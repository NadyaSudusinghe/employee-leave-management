using System.ComponentModel.DataAnnotations;

namespace LeaveManagement.Api.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required] [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required] [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        public DateTime DateJoined { get; set; } = DateTime.UtcNow;
    }
}
