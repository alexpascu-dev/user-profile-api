namespace WebAPI.Models;

public class ChangePasswordAsync
{
    public int UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}