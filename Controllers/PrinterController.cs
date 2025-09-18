using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Contracts;
using WebAPI.Models;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class PrinterController : ControllerBase
{
    private readonly IPrinterService _printerService;

    public PrinterController(IPrinterService printerService)
    {
        _printerService = printerService;
    }
    
    //GET Printer info
    [HttpGet("info")]
    [SwaggerOperation(Summary = "Get printer info", Tags = new[] { "Printer" })]

    public async Task<ActionResult<PrinterInfoDto>> GetPrinterInfo()
    {
        try
        {
            var info = await _printerService.GetPrinterInfoAsync();
            return info is null ? NoContent() : Ok(info);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    //POST Printer settings
    [HttpPost("mac")]
    [SwaggerOperation(Summary = "Save printer mac", Tags = new[] { "Printer" })]

    public async Task<IActionResult> SaveMacAsync([FromBody] SavePrinterMacDto dto)
    {
        try
        {
            await _printerService.SaveMacAsync(dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}