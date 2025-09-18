namespace WebAPI.Models;

public class PrinterInfoDto
{
    public int Id { get; set; }
    public string Mac { get; set; } = default!;
    public string? Name { get; set; }
}