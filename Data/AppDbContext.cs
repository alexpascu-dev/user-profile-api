using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Identity;
using Constants = WebAPI.Helper.Constants;

namespace WebAPI.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //ROLES TABLE

        //ADMIN
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = Constants.AdminRoleId,
                Name = Constants.RoleAdmin,
                NormalizedName = Constants.RoleAdmin.ToUpper(),
                ConcurrencyStamp = "ROLE_ADMIN_STAMP"
            },
            //SUPERVISOR
            new IdentityRole
            {
                Id = Constants.SupervisorRoleId,
                Name = Constants.RoleSupervisor,
                NormalizedName = Constants.RoleSupervisor.ToUpper(),
                ConcurrencyStamp = "ROLE_SUPERVISOR_STAMP"
            },
            //BASIC USER
            new IdentityRole
            {
                Id = Constants.BasicRoleId,
                Name = Constants.RoleBasicUser,
                NormalizedName = Constants.RoleBasicUser.ToUpper(),
                ConcurrencyStamp = "ROLE_BASIC_STAMP"
            }
        );

        //ROLE CLAIMS TABLE
        builder.Entity<IdentityRoleClaim<string>>().HasData(
            
            //admin
            new IdentityRoleClaim<string> 
            {
                Id = 1, RoleId = Constants.AdminRoleId, ClaimType = Constants.ClaimTypePermission,
                ClaimValue = Constants.PermUsersRead
            },
            new IdentityRoleClaim<string>
            {
                Id = 2, RoleId = Constants.AdminRoleId, ClaimType = Constants.ClaimTypePermission, 
                ClaimValue = Constants.PermUsersWrite
            },
            new IdentityRoleClaim<string>
            {
                Id = 3, RoleId = Constants.AdminRoleId, ClaimType = Constants.ClaimTypePermission, 
                ClaimValue = Constants.PermUsersDelete
            },
            //supervisor
            new IdentityRoleClaim<string>
            {
                Id = 4, RoleId = Constants.SupervisorRoleId, ClaimType = Constants.ClaimTypePermission,
                ClaimValue = Constants.PermUsersRead
            },
            new IdentityRoleClaim<string>
            {
                Id = 5, RoleId = Constants.SupervisorRoleId, ClaimType = Constants.ClaimTypePermission, 
                ClaimValue = Constants.PermUsersWrite
            },
            //basic user
            new IdentityRoleClaim<string>
            {
                Id = 6, RoleId = Constants.BasicRoleId, ClaimType = Constants.ClaimTypePermission, ClaimValue = Constants.PermUsersRead
            }
        );
        
        // Default USERS
        var hasher = new PasswordHasher<User>();

        var adminUser = new User()
        {
            Id = Constants.AdminUserId,
            UserName = "admin",
            NormalizedUserName = "admin".ToUpper(),
            Email = "admin@test.local",
            NormalizedEmail = "admin@test.local".ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "ADMIN_SECURITY_STAMP",
            ConcurrencyStamp = "ROLE_ADMIN_STAMP",
            PasswordHash = "AQAAAAEAACcQAAAAEPG1aqOdTnqTePFq+aV0I132LAb3g/AaBAWnqvQAKrSK+JuW+FO02zF3vKU21bx+dw=="
        };
        // adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin123!@");

        var supervisorUser = new User()
        {
            Id = Constants.SupervisorUserId,
            UserName = "supervisor",
            NormalizedUserName = "supervisor".ToUpper(),
            Email = "supervisor@test.local",
            NormalizedEmail = "supervisor@test.local".ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "SUP_SECURITY_STAMP",
            ConcurrencyStamp = "ROLE_SUPERVISOR_STAMP",
            PasswordHash = "AQAAAAEAACcQAAAAEDQXfPBNIWeBqiR876JB17H8NCJ6CGDLQrKzKn7/SNC8LX8nyGN15oSOJsDuj4MFbg=="
        };
         //supervisorUser.PasswordHash = hasher.HashPassword(supervisorUser, "Supervisor123!@");

        var basicUser = new User()
        {
            Id = Constants.BasicUserId,
            UserName = "basic",
            NormalizedUserName = "basic".ToUpper(),
            Email = "basic@test.local",
            NormalizedEmail = "basic@test.local".ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "BASIC_SECURITY_STAMP",
            ConcurrencyStamp = "ROLE_BASIC_STAMP",
            PasswordHash = "AQAAAAEAACcQAAAAEM7sh9VP7kuA40DXx+xvNpVquMhRLvaT4obbaDV/OzLAsW1DRoTL+dHfUOezOgWSQQ=="
        };
        // basicUser.PasswordHash = hasher.HashPassword(basicUser, "Basic123!@");

        builder.Entity<User>().HasData(adminUser, supervisorUser, basicUser);

        //UserId -> RolesId
        builder.Entity<IdentityUserRole<string>>().HasData(
            //ADMIN
            new IdentityUserRole<string> { UserId = Constants.AdminUserId, RoleId = Constants.AdminRoleId },
            new IdentityUserRole<string> { UserId = Constants.SupervisorUserId, RoleId = Constants.SupervisorRoleId },
            new IdentityUserRole<string> { UserId = Constants.BasicUserId, RoleId = Constants.BasicRoleId }
        );
    }
}