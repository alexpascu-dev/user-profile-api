using Microsoft.Build.Framework;

namespace WebAPI.Models;

public class SavePrinterMacDto
{
    [Required]
    public string Mac { get; set; } = default!;
    public string? Name { get; set; }
}