namespace LeaveManagement.Api.DTOs
{
    public class EmployeeReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
    }
}
