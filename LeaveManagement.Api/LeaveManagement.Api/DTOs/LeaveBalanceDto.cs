namespace LeaveManagement.Api.DTOs
{
    public class LeaveBalanceDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int AnnualLeaveUsed { get; set; }
        public int AnnualLeaveRemaining { get; set; }
        public int CasualLeaveUsed { get; set; }
        public int CasualLeaveRemaining { get; set; }
    }
}
