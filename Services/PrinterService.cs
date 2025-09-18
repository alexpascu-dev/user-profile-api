using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
using WebAPI.Contracts;
using WebAPI.Models;

namespace WebAPI.Services;

public class PrinterService : IPrinterService
{
    private readonly string _connectionString;
    private readonly ILogger<UserService> _logger;

    public PrinterService(IConfiguration configuration, ILogger<UserService> logger)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("No connection string configured");
    }

//GET Printer INFO
    public async Task<PrinterInfoDto?> GetPrinterInfoAsync()
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                    SELECT TOP 1 Mac, Name
                    FROM Dbo.PrinterInfo
                    ORDER BY Id DESC;
                ";
                
                var info = await conn.QueryFirstOrDefaultAsync<PrinterInfoDto>(query);
                return info;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    
//POST Printer Settings
    public async Task SaveMacAsync(SavePrinterMacDto dto)
    {
        try
        {
            var mac = NormalizeMac(dto.Mac);
            if (!IsValidMac(mac))
                throw new InvalidOperationException("MAC invalid. Format: AA:BB:CC:DD:EE:FF");

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                IF EXISTS (SELECT 1 FROM dbo.PrinterInfo)
                    UPDATE dbo.PrinterInfo
                    SET Mac = @Mac, Name = @Name
                    WHERE ID = (SELECT TOP 1 ID FROM dbo.PrinterInfo ORDER BY ID DESC);
                ELSE 
                    INSERT INTO dbo.PrinterInfo (Mac, Name) VALUES (@Mac, @Name);
                ";
                
                await conn.ExecuteAsync(query, new { Mac = mac, Name = dto.Mac });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    private static string NormalizeMac(string raw)
    {
        var hex = new string(raw.Where(Uri.IsHexDigit).ToArray()).ToUpperInvariant();
        if (hex.Length != 12) return raw;
        return string.Join(":", Enumerable.Range(0, 6).Select(i => hex.Substring(i * 2, 2)));
    }

    private static bool IsValidMac(string mac) =>
        Regex.IsMatch(mac, @"^([0-9A-F]{2}:){5}[0-9A-F]{2}$");
}