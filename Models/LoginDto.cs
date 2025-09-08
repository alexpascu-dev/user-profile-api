using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class LoginDto
{
    [Required, MinLength(3)]
    public string UserName { get; set; }  = string.Empty;
    
    [Required, MinLength(5)]
    public string Password { get; set; }  = string.Empty;
}