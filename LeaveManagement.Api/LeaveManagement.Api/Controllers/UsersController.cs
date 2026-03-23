using LeaveManagement.Api.Common;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> LinkUserToEmployee(LinkUserEmployeeDto dto)
        {
            var result = await _userService.LinkUserToEmployee(dto.UserId, dto.EmployeeId);

            if (!result)
                return NotFound("User or Employee not found");

            return Ok("Linked sucessfully!");
        }
    }
}
