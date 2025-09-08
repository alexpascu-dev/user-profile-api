using Microsoft.AspNetCore.Identity;

namespace WebAPI.Models;

public class GetUserDto
{
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Role { get; set; }
}