using LeaveManagement.Api.DTOs;

namespace LeaveManagement.Api.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeReadDto> CreateEmployee(EmployeeCreateDto dto);
        Task<IEnumerable<EmployeeReadDto>> GetAllEmployees();
        Task<EmployeeReadDto?> GetEmployeeById(int id);
        Task<bool> UpdateEmployee(int id, EmployeeUpdateDto dto);
        Task<bool> DeleteEmployee(int id);
    }
}
