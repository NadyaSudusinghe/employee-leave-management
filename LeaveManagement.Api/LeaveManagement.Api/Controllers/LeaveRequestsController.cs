using LeaveManagement.Api.Common;
using LeaveManagement.Api.Data;
using LeaveManagement.Api.DTOs;
using LeaveManagement.Api.Models;
using LeaveManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeaveManagement.Api.Controllers
{
    [Route("api/leaverequests")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestsController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest(LeaveRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var employeeId = GetEmployeeIdFromToken();

            try
            {
                if (role != Roles.Admin)
                {
                    if (employeeId == null || dto.EmployeeId != employeeId)
                        return Forbid();
                }
                var leaveRequest = await _leaveRequestService.CreateLeaveRequest(dto);
                return Ok(leaveRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var leaveRequests = await _leaveRequestService.GetAllLeaveRequests();

            return Ok(leaveRequests);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequestById(int id)
        {
            var leaveRequest = await _leaveRequestService.GetLeaveRequestById(id);

            if (leaveRequest == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeeId = GetEmployeeIdFromToken();

            if (role != Roles.Admin && leaveRequest.EmployeeId != employeeId)
                return Forbid();

            return Ok(leaveRequest);
        }

        [Authorize]
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetLeaveRequestsByEmployeeId(int employeeId)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var tokenEmployeeId = GetEmployeeIdFromToken();

                if(role != Roles.Admin)
                {
                    if (tokenEmployeeId == null || tokenEmployeeId != employeeId)
                        return Forbid();
                }

                var leaveRequests = await _leaveRequestService.GetLeaveRequestsByEmployeeId(employeeId);
                return Ok(leaveRequests);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var employeeId = GetEmployeeIdFromToken();

                if (role != Roles.Admin)
                {
                    if (employeeId == null || dto.EmployeeId != employeeId)
                        return Forbid();
                }

                var updated = await _leaveRequestService.UpdateLeaveRequest(id, dto);
                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateLeaveRequestStatus(int id, LeaveRequestStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _leaveRequestService.UpdateLeaveRequestStatus(id, dto.Status);
                if (!updated)
                    return NotFound("Status update unsucessfull");

                return Ok("Status updated successfully!");
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            var deleted = await _leaveRequestService.DeleteLeaveRequest(id);
            if (!deleted)
                return NotFound();
            
            return NoContent();
        }

        private int? GetEmployeeIdFromToken()
        {
            var claim = User.FindFirst("employeeId")?.Value;

            if (string.IsNullOrEmpty(claim))
                return null;

            return int.Parse(claim);
        }
    }
}
