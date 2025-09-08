using WebAPI.Models;

namespace WebAPI.Contracts;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginDto loginDto); 
}