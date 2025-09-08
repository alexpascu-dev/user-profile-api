using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class CreateUserDto
{
    [Required, MinLength(3)]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(5)]
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    [Required]
    public string Role { get; set; } = string.Empty;
}