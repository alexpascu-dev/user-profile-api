using Microsoft.AspNetCore.Identity;
using WebAPI.Contracts;
using WebAPI.Models;
using WebAPI.Models.Identity;

namespace WebAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenProvider _jwt;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<User> userManager, IJwtTokenProvider jwtTokenProvider, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwt = jwtTokenProvider;
        _logger = logger;
    }

    //LOGIN
    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var identityUser = await _userManager.FindByNameAsync(loginDto.UserName);
            if (identityUser is null)
                return null;
            
            var ok = await _userManager.CheckPasswordAsync(identityUser, loginDto.Password);
            if (!ok)
                return null;
            
            var token = await _jwt.Create(identityUser);
            
            return token;
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Username}", loginDto.UserName);
            throw;
        }
    }
}