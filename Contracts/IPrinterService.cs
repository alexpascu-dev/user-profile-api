using WebAPI.Models;

namespace WebAPI.Contracts;

public interface IPrinterService
{
    Task<PrinterInfoDto?> GetPrinterInfoAsync();

    Task SaveMacAsync(SavePrinterMacDto dto);
}