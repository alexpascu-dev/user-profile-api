using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class GetUseRolesDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}