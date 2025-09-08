namespace WebAPI.Helper;

public static class Constants
{
    //JWT
    public const int ExpirationToken = 15;

    //Roles
    public const string RoleBasicUser = "USER";
    public const string RoleSupervisor = "SUPERVISOR";
    public const string RoleAdmin = "ADMIN";
    
    //Roles id
    public const string AdminRoleId = "role-admin-id";
    public const string SupervisorRoleId = "supervisor-role-id";
    public const string BasicRoleId = "basic-role-id";
    
    //Users
    public const string AdminUserId = "user-admin-id";
    public const string SupervisorUserId = "user-supervisor-id";
    public const string BasicUserId = "user-basic-id";
    
    //Claim type
    public const string ClaimTypePermission = "permission";
    
    //Permissions
    public const string PermUsersRead = "users.read"; // read
    public const string PermUsersWrite = "users.write"; // create, read, delete
    public const string PermUsersDelete = "users.delete"; // delete
}