namespace LeaveManagement.Api.DTOs
{
    public class LeaveRequestCreateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
    }
}
