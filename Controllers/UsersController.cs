using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contracts;
using WebAPI.Helper;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
// localhost:xxxx/api/users
[Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/users
    [Authorize(Roles = Constants.RoleAdmin +  "," + Constants.RoleSupervisor)]
    [HttpGet]
    [SwaggerOperation(Summary = "Get all users", Tags = new[] {"Users - GET"})]
    
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // GET api/users - SORTED, FILTERED, PAGINATION
    [Authorize(Roles = Constants.RoleAdmin +  "," + Constants.RoleSupervisor)]
    [HttpGet("all-users")]
    [SwaggerOperation(Summary = "Get users (paged/filter/sort)", Tags = new[] { "Users - GET" })]

    public async Task<ActionResult<PagedResult<GetUserDto>>> GetUsersAsync([FromQuery] PagedUsersQuery query)
    {
        try
        {
            var result = await _userService.GetUsersAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //GET (One user by ID)
    [HttpGet("{userId:int}")]
    [SwaggerOperation(Summary = "Get user by ID", Tags = new[] {"Users - GET"})]
    
    public async Task<IActionResult> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();
            else
                return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    //GET (One user by Username)
    [HttpGet("{username}")]
    [SwaggerOperation(Summary = "Get user by username", Tags = new[] {"Users - GET"})]
    
    public async Task<IActionResult> GetUserByUsernameAsync(string username)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound();
            else
                return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    //GET (User info from TOKEN BODY)
    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Get user info from jwt token body", Tags = new[] {"Users - GET"})]
    
    public async Task<IActionResult> GetCurrentUserAsync()
    {
        try
        {
            var identityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            if (string.IsNullOrEmpty(identityUserId))
                return Unauthorized(new { message = "Invalid token" });
            
            var dto = await _userService.GetCurrentUserAsync(identityUserId);
            if (dto is null)
                return NotFound();
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET ROLES
    [HttpGet("roles")]
    [Authorize(Roles = Constants.RoleAdmin)]
    [SwaggerOperation(Summary = "Get user roles", Tags = new[] {"Users - GET"})]
    
    public async Task<IActionResult> GetUserRolesAsync()
    {
        try
        {
            var roles = await _userService.GetUserRolesAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // POST / REGISTER / CREATE
    [Authorize(Roles = Constants.RoleAdmin)]
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register/create user", Tags = new[] {"Users - POST"})]
    
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto dto)
    {
        try
        {
            var userId = await _userService.CreateUserAsync(dto);
            if (userId == 0)
                return NotFound();

            return Ok(new { userId = userId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

//PUT / UPDATE
    [Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleSupervisor)]
    [HttpPut("update")]
    [SwaggerOperation(Summary = "Update user information", Tags = new[] {"Users - UPDATE"})]
    
    public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserDto dto)
    {
        try
        {
            var affectedRows = await _userService.UpdateUserAsync(dto);
            if (affectedRows == 0)
                return NotFound();
    
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // CHANGE PASSWORD
    [Authorize(Roles = Constants.RoleAdmin)]
    [HttpPut("update/change-password")]
    [SwaggerOperation(Summary = "Admin can Change User password", Tags = new[] {"Users - UPDATE"})]

    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordAsync request)
    {
        try
        {
            if (request is null || request.UserId <= 0 || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { error = "UserId and Password are required" });
            
            await _userService.ChangePasswordAsync(request.UserId, request.NewPassword);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE
    [Authorize(Roles = Constants.RoleAdmin)]
    [HttpDelete("{userId:int}")]
    [SwaggerOperation(Summary = "Delete user by userId", Tags = new[] {"Users - DELETE"})]
    
    public async Task<IActionResult> DeleleUserAsync(int userId)
    {
        try
        {
            var status = await _userService.DeleteUserAsync(userId);
            if (status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }    
    
    // CHANGE ROLE
    [Authorize(Roles = Constants.RoleAdmin)]
    [HttpPost("change-role")]
    
    public async Task<IActionResult> ChangeUserRoleAsync(string username, string newRole)
    {
        try
        {
            var result = await _userService.ChangeUserRoleAsync(username, newRole);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // GET (users report preview by date range)
    [Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleSupervisor)]
    [HttpGet("report-preview")]
    [SwaggerOperation(Summary = "Get users report preview", Tags = new[] {"Users - GET"})]
    public async Task<IActionResult> GetUsersReportPreviewAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var users = await _userService.GetUsersReportPreviewAsync(startDate, endDate);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    // GET (export users report)
    [Authorize(Roles = Constants.RoleAdmin + "," + Constants.RoleSupervisor)]
    [HttpGet("export-report")]
    [SwaggerOperation(Summary = "Export users report", Tags = new[] {"Users - GET"})]
    public async Task<IActionResult> ExportUsersReportAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        try
        {
            var (fileBytes, fileName) = await _userService.ExportUsersReportAsync(startDate, endDate);
            return File(fileBytes, "text/csv", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}