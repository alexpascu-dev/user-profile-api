using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class UpdateUserDto
{
    //IDENTITY AspNetUsers
    public int UserId {get; set;}
    [Required, MinLength(3)]
    public string? Username {get; set;} = string.Empty;
    [Required]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    //DAPPER Users
    [Required]
    public string? FirstName {get; set;} = string.Empty;
    [Required]
    public string? LastName {get; set;} =string.Empty;
    public bool? IsActive {get; set;}
    public string? Role { get; set;}
    public string? NewPassword {get; set;}
}