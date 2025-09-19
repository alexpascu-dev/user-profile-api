using WebAPI.Models;

namespace WebAPI.Contracts;

public interface IUserService
{
    // GET (all USERS)
    Task<IEnumerable<GetUserDto>>GetAllUsersAsync();
    
    // GET (all USERS sorted / filtered)
    Task<PagedResult<GetUserDto>> GetUsersAsync(PagedUsersQuery query);
    
    // GET (one USER)
    Task<GetUserDto?> GetUserByIdAsync(int userId);
    
    // GET (user by USERNAME)
    Task<GetUserDto?> GetUserByUsernameAsync(string username);
    
    // GET ( USER INFO from TOKEN)
    Task<GetUserDto?> GetCurrentUserAsync(string identityUserId);
    
    // POST (CREATE)
    Task<int> CreateUserAsync(CreateUserDto dto);
    
    // PUT (UPDATE)
    Task<int> UpdateUserAsync(UpdateUserDto dto);
    
    //CHANGE PASSWORD
    Task ChangePasswordAsync(int userId, string newPassword);
    
    // DELETE
    Task<bool> DeleteUserAsync(int userId);
    
    // GET ROLES
    Task<IEnumerable<GetUseRolesDto>> GetUserRolesAsync();
    
    // CHANGE ROLE
    Task<string> ChangeUserRoleAsync(string username, string newRole);
    
    // GET (users report preview by date range)
    Task<IEnumerable<CsvUserDto>> GetUsersReportPreviewAsync(DateTime startDate, DateTime endDate);

    // GET (export users report)
    Task<(byte[], string)> ExportUsersReportAsync(DateTime startDate, DateTime endDate);
}