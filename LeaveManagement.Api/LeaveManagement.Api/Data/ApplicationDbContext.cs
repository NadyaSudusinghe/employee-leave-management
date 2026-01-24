using Microsoft.EntityFrameworkCore;
using LeaveManagement.Api.Models;

namespace LeaveManagement.Api.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
