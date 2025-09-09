using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using WebAPI.Contracts;
using WebAPI.Helper;
using WebAPI.Models;
using WebAPI.Models.Identity;

namespace WebAPI.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly string _connectionString;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No connection string configured");
    }

    // GET (ALL users)
    public async Task<IEnumerable<GetUserDto>> GetAllUsersAsync()
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    --language=sql
                SELECT 
                  u.UserId,
                  a.UserName AS Username, 
                  u.FirstName,
                  u.LastName,
                  a.Email,
                  u.IsActive,
                  u.CreatedDate,
                  r.Name AS Role
                FROM Users u
                JOIN AspNetUsers a ON a.Id = u.IdentityUserId
                JOIN AspNetUserRoles ur ON a.Id = ur.UserId
                JOIN AspNetRoles r ON ur.RoleId = r.Id
                ";

                var users = await conn.QueryAsync<GetUserDto>(query);
                return users;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    // GET (ALL users + Search, Pagination, Filter)
    public async Task<PagedResult<GetUserDto>> GetUsersAsync(PagedUsersQuery query)
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                var sortMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    // ["userId"] = "u.UserId",
                    // ["userName"] = "a.UserName",
                    // ["firstName"] = "u.FirstName",
                    // ["lastName"] = "u.LastName",
                    // ["email"] = "a.Email",
                    // ["isActive"] = "u.IsActive",
                    // ["createdDate"] = "u.CreatedDate",
                    // ["role"] = "r.Name"
                    
                    ["userId"] = "UserId",
                    ["userName"] = "UserName",
                    ["firstName"] = "FirstName",
                    ["lastName"] = "LastName",
                    ["email"] = "Email",
                    ["isActive"] = "IsActive",
                    ["createdDate"] = "CreatedDate",
                    ["role"] = "Role"
                };

                var sortBy = sortMap.ContainsKey(query.SortBy ?? "") ? sortMap[query.SortBy!] : "CreatedDate";
                var sortDir = string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

                var offset = query.PageIndex * query.PageSize;
                var p = new DynamicParameters();
                p.Add("@offset", offset);
                p.Add("@pageSize", query.PageSize);
                p.Add("@isActive", query.IsActive);
                p.Add("@query", string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search!.Trim()}%");

                // Search + Filter (active / inactive)
                var where = @"
        WHERE
            (@query IS NULL OR a.UserName LIKE @query OR a.Email LIKE @query OR u.FirstName LIKE @query OR u.LastName LIKE @query)
            AND (@isActive IS NULL OR @isActive = u.IsActive)";

                var roleFrom = $@"
        FROM Users u
        JOIN AspNetUsers a ON a.Id = u.IdentityUserId
        JOIN AspNetUserRoles ur ON a.Id = ur.UserId
        JOIN AspNetRoles r ON ur.RoleId = r.Id
        {where}";

                var dataSql = $@"
        ;WITH base AS (
            SELECT 
                u.UserId,
                a.UserName AS Username, 
                u.FirstName,
                u.LastName,
                a.Email,
                u.IsActive,
                u.CreatedDate,
                r.Name AS Role
            {roleFrom}
        )

        SELECT * INTO #base FROM base;

        SELECT * FROM #base
        ORDER BY {sortBy} {sortDir}
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;

        SELECT COUNT(*) FROM #base;";
                
                var multipleResults = await conn.QueryMultipleAsync(dataSql, p);
                
                var users = (await multipleResults.ReadAsync<GetUserDto>()).ToList();
                var totalPages = await multipleResults.ReadFirstAsync<int>();

                return new PagedResult<GetUserDto>
                {
                    Items = users,
                    Total = totalPages,
                    PageIndex = query.PageIndex,
                    PageSize = query.PageSize
                };
            }
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    // GET USER BY ID
    public async Task<GetUserDto?> GetUserByIdAsync(int userId)
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                SELECT 
                  u.UserId,
                  a.UserName AS Username,
                  u.FirstName,
                  u.LastName,
                  a.Email,
                  u.IsActive,
                  u.CreatedDate,
                  r.Name AS Role
                FROM Users u
                JOIN AspNetUsers a ON a.Id = u.IdentityUserId
                JOIN AspNetUserRoles ur ON a.Id = ur.UserId
                JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE u.UserId = @userId";

                var user = await conn.QueryFirstOrDefaultAsync<GetUserDto>(query, new { userId = userId });
                if (user == null)
                    return null;
                else
                    return user;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user");
            throw;
        }
    }
    
    // GET USER BY USERNAME
    public async Task<GetUserDto?> GetUserByUsernameAsync(string username)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            const string sql = @"
                --language=sql
                SELECT 
                  u.UserId,
                  a.UserName AS Username,
                  u.FirstName,
                  u.LastName,
                  a.Email,
                  u.IsActive,
                  u.CreatedDate,
                  r.Name AS Role
                FROM Users u
                JOIN AspNetUsers a ON a.Id = u.IdentityUserId
                JOIN AspNetUserRoles ur ON a.Id = ur.UserId
                JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE a.UserName = @username";
            return await conn.QueryFirstOrDefaultAsync<GetUserDto>(sql, new { username });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by username");
            throw;
        }
    }
    
    // GET USER FROM TOKEN
    public async Task<GetUserDto?> GetCurrentUserAsync(string identityUserId)
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                SELECT 
                    u.UserId, 
                    u.FirstName, 
                    u.LastName, 
                    u.IsActive, 
                    u.CreatedDate,
                    a.UserName AS Username,
                    a.Email,
                    r.Name AS Role
                FROM Users u
                JOIN AspNetUsers a ON a.Id = u.IdentityUserId
                JOIN AspNetUserRoles ur ON a.Id = ur.UserId
                JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE u.IdentityUserId = @IdentityUserId";
                
                return await conn.QueryFirstOrDefaultAsync<GetUserDto>(query, new { IdentityUserId = identityUserId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user information");
            throw;
        }
    }
    
    //GET ROLES
    public async Task<IEnumerable<GetUseRolesDto>> GetUserRolesAsync()
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                SELECT NAME AS Role FROM dbo.AspNetRoles";

                var roles = await conn.QueryAsync<GetUseRolesDto>(query);
                return roles;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            throw;
        }
    }

    //CREATE USER (POST)
    public async Task<int> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            if (await _userManager.FindByNameAsync(dto.Username) is not null)
                throw new InvalidOperationException("Username already exists");
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                throw new InvalidOperationException("Email already exists");
            
            string roleToAsign;
            if (string.IsNullOrWhiteSpace(dto.Role))
                roleToAsign = Constants.RoleBasicUser;
            else
                roleToAsign = dto.Role!;
            
            roleToAsign = roleToAsign.ToUpperInvariant();
            
            if (!await _roleManager.RoleExistsAsync(roleToAsign))
                throw new InvalidOperationException("Role does not exist");
            
            var identityUser = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = true
            };
            
            var createResult = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!createResult.Succeeded)
                throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(error => error.Description)));
            
            try
            {
                var addRole = await _userManager.AddToRoleAsync(identityUser, roleToAsign);
                if (!addRole.Succeeded)
                    throw new InvalidOperationException("Error adding user role.");
                
                using (IDbConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"
                --language=sql
                    INSERT INTO dbo.Users 
                        (IdentityUserId, FirstName, LastName, IsActive)
                    VALUES
                        (@IdentityUserId, @FirstName, @LastName, @IsActive);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var userId = await conn.ExecuteScalarAsync<int>(query, new
                    {
                        IdentityUserId = identityUser.Id,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        IsActive = dto.IsActive
                    });

                    return userId;
                }
            }
            catch
            {
                await _userManager.DeleteAsync(identityUser);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating user {username}", dto.Username);
            throw;
        }
    }
    
    //PUT / UPDATE
    public async Task<int> UpdateUserAsync(UpdateUserDto dto)
    {
        try
        {
            if (dto.UserId < 0)
                throw new ArgumentException("UserId is required");

            string? identityUserId;
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --lanuage=sql
                SELECT IdentityUserId 
                    FROM dbo.Users 
                WHERE UserId = @UserId
                ";
                identityUserId = await conn.ExecuteScalarAsync<string?>(query, new { UserId = dto.UserId });
            }

            if (string.IsNullOrWhiteSpace(identityUserId))
                throw new InvalidOperationException("No identity user found");

            var user = await _userManager.FindByIdAsync(identityUserId) ??
                       throw new InvalidOperationException("No user found");

            //USERNAME
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                var exists =
                    await _userManager.Users.AnyAsync(u => u.UserName == dto.Username && u.Id != user.Id);
                if (exists)
                    throw new InvalidOperationException("Username already exists");
                var setUsername = await _userManager.SetUserNameAsync(user, dto.Username);
                if (!setUsername.Succeeded)
                    throw new InvalidOperationException("Failed to set username");
            }

            //EMAIL
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var exists = await _userManager.Users.AnyAsync(u => u.Email == dto.Email && u.Id != user.Id);
                if (exists)
                    throw new InvalidOperationException("Email already exists");

                var setEmail = await _userManager.SetEmailAsync(user, dto.Email);
                if (!setEmail.Succeeded)
                    throw new InvalidOperationException("Failed to set email");

                user.EmailConfirmed = true;

                var updateEmail = await _userManager.UpdateAsync(user);
                if (!updateEmail.Succeeded)
                    throw new InvalidOperationException("Failed to update email");
            }

            //ROLE
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                var newRole = dto.Role;
                
                if (!await _roleManager.RoleExistsAsync(newRole))
                    throw new InvalidOperationException("Role does not exist");

                var currentRoles = await _userManager.GetRolesAsync(user);
                
                var alreadyAssignedRoles = currentRoles.Any(role => role == newRole);
                if (!alreadyAssignedRoles)
                {
                    var addRole = await _userManager.AddToRoleAsync(user, newRole);
                    if (!addRole.Succeeded)
                        throw new InvalidOperationException("Error adding new role");                
                
                    var rolesToBeRemoved = currentRoles.Where(role => role != newRole).ToList();
                        
                    var removeOtherRoles = await _userManager.RemoveFromRolesAsync(user, rolesToBeRemoved);
                    if (!removeOtherRoles.Succeeded) 
                        throw new InvalidOperationException("Error removing rest of roles");
                }
            }

            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                --language=sql
                UPDATE dbo.Users
                SET
                FirstName = COALESCE(@FirstName, FirstName),
                LastName = COALESCE(@LastName, LastName),
                IsActive = COALESCE(@IsActive, IsActive)
                WHERE UserId = @UserId;
                ";

                var rowsAffected = await conn.ExecuteAsync(query, dto);
                if (rowsAffected == 0)
                    throw new InvalidOperationException("No rows affected");
                return rowsAffected;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating user");
            throw;
        }
    }
    
    // CHANGE PASSWORD
    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required");

            IDbConnection conn = new SqlConnection(_connectionString);
            
            const string query = @"
            --language=sql
            SELECT IdentityUserId
            FROM dbo.Users
            WHERE UserId = @UserId
            ";

                var identityUserId = await conn.ExecuteScalarAsync<string?>(query, new { UserId = userId });

                if (string.IsNullOrWhiteSpace(identityUserId))
                    throw new InvalidOperationException("No identity user found");

                var user = await _userManager.FindByIdAsync(identityUserId) ??
                           throw new InvalidOperationException("No user found");

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordReset = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!passwordReset.Succeeded)
                    throw new InvalidOperationException("Failed to change password");
                
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating user password");
            throw;
        }
    }
    
    // DELETE
    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
            --language=sql
            DELETE a
            FROM AspNetUsers a
            JOIN Users u ON a.Id = u.IdentityUserId
            WHERE u.UserId = @UserId";

                var rowsAffected = await conn.ExecuteAsync(query, new { UserId = userId });
                if (rowsAffected > 0)
                    return true;
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            throw;
        }
    }
    
    // CHANGE ROLE 
    public async Task<string> ChangeUserRoleAsync(string username, string newRole)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username) ??
                       throw new InvalidOperationException("User not found");
            
            if (!await _roleManager.RoleExistsAsync(newRole))
                throw new InvalidOperationException("Role does not exist");
            
            var currentRoles = await _userManager.GetRolesAsync(user);
            
            if (currentRoles.Count == 1 && currentRoles.Contains(newRole))
                return ("Role is already assigned");

            if (currentRoles.Count > 0)
            {
                var removeRole = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if  (!removeRole.Succeeded)
                    throw new InvalidOperationException("Error removing role");
            }
            
            var addRole = await _userManager.AddToRoleAsync(user, newRole);
            if (!addRole.Succeeded)
                throw new InvalidOperationException("Error adding new role");
            
            return ($"Role {newRole} is now assigned");
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing user role");
            throw;
        }
    }   
}



