using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Contracts;
using WebAPI.Models.Identity;
using Constants = WebAPI.Helper.Constants;

namespace WebAPI.Helper;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    

    public JwtTokenProvider(
        IConfiguration configuration,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<string> Create(User user)
    {
        string secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        //ROLES
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
            claims.Add(new Claim("role", userRole));
        
        //PERMISSION ROLES
        foreach (var userRole in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(userRole);
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            
            foreach (var roleClaim in roleClaims)
                claims.Add(roleClaim);
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Constants.ExpirationToken),
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        
        var handler = new JsonWebTokenHandler();
        
        string token = handler.CreateToken(tokenDescriptor);
        
        return token;
    }
}