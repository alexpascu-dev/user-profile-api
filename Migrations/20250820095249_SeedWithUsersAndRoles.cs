using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedWithUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "basic-role-id", "ROLE_BASIC_STAMP", "USER", "USER" },
                    { "role-admin-id", "ROLE_ADMIN_STAMP", "ADMIN", "ADMIN" },
                    { "supervisor-role-id", "ROLE_SUPERVISOR_STAMP", "SUPERVISOR", "SUPERVISOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user-admin-id", 0, "ROLE_ADMIN_STAMP", "admin@test.local", true, false, null, "ADMIN@TEST.LOCAL", "ADMIN", "AQAAAAEAACcQAAAAEPG1aqOdTnqTePFq+aV0I132LAb3g/AaBAWnqvQAKrSK+JuW+FO02zF3vKU21bx+dw==", null, false, "ADMIN_SECURITY_STAMP", false, "admin" },
                    { "user-basic-id", 0, "ROLE_BASIC_STAMP", "basic@test.local", true, false, null, "BASIC@TEST.LOCAL", "BASIC", "AQAAAAEAACcQAAAAEM7sh9VP7kuA40DXx+xvNpVquMhRLvaT4obbaDV/OzLAsW1DRoTL+dHfUOezOgWSQQ==", null, false, "BASIC_SECURITY_STAMP", false, "basic" },
                    { "user-supervisor-id", 0, "ROLE_SUPERVISOR_STAMP", "supervisor@test.local", true, false, null, "SUPERVISOR@TEST.LOCAL", "SUPERVISOR", "AQAAAAEAACcQAAAAEDQXfPBNIWeBqiR876JB17H8NCJ6CGDLQrKzKn7/SNC8LX8nyGN15oSOJsDuj4MFbg==", null, false, "SUP_SECURITY_STAMP", false, "supervisor" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permission", "users.read", "role-admin-id" },
                    { 2, "permission", "users.write", "role-admin-id" },
                    { 3, "permission", "users.read", "supervisor-role-id" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "role-admin-id", "user-admin-id" },
                    { "basic-role-id", "user-basic-id" },
                    { "supervisor-role-id", "user-supervisor-id" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "role-admin-id", "user-admin-id" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "basic-role-id", "user-basic-id" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "supervisor-role-id", "user-supervisor-id" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "basic-role-id");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "role-admin-id");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "supervisor-role-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-admin-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-basic-id");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-supervisor-id");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
