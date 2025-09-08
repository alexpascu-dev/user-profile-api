using WebAPI.Models.Identity;

namespace WebAPI.Contracts;

public interface IJwtTokenProvider
{
    Task<string> Create(User user);
}