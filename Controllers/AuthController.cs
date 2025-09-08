using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Contracts;
using WebAPI.Helper;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    
    // POST / LOGIN API
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var token = await _authService.LoginAsync(loginDto);
            if (token is null)
                return Unauthorized(new { message = "Username or password is incorrect" });
            
            // FOR ANDROID/ANGULAR
            // var profile = await _userService.GetUserByUsernameAsync(loginDto.UserName);
            // if (profile == null)
            //     return NotFound(new
            //     { message = "Username is incorrect" });

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}