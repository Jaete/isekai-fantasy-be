using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Services;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IsekaiFantasyBE.Migrations
{
    public partial class _001regularuserandadmin : Migration
    {
        private static readonly Guid AdminId = Guid.Parse("A0A0A0A0-A0A0-A0A0-A0A0-A0A0A0A0A0A0");
        private static readonly Guid UserId = Guid.Parse("B0B0B0B0-B0B0-B0B0-B0B0-B0B0B0B0B0B0");
        private static readonly string AdminPass = "ADMIROadmiro123!!";
        private static readonly string UserPass = "USERuser123!!";
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]{"Id", "Username", "Email", "Password", "CreatedAt", "UpdatedAt", "LastLogin"},
                values: new object[]
                {
                    AdminId,
                    "Admin",
                    "admin@user.com",
                    PasswordService.Encrypt(AdminPass),
                    DateTime.Now,
                    DateTime.Now,
                    DateTime.Now,
                }
            );
            
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]{"Id", "Username", "Email", "Password", "CreatedAt", "UpdatedAt", "LastLogin"},
                values: new object[]
                {
                    UserId,
                    "User",
                    "user@user.com",
                    PasswordService.Encrypt(UserPass),
                    DateTime.Now,
                    DateTime.Now,
                    DateTime.Now,
                }
            );
            
            migrationBuilder.InsertData(
                table: "UsersProperties",
                columns: new[]{"UserId", "Photo", "Bio", "Status", "UserRole", "LastActivity"},
                values: new object[]
                {
                    AdminId,
                    null,
                    null,
                    UserProperties.ACTIVE,
                    (int)UserProperties.Role.Admin,
                    DateTime.Now,
                }
            );
            
            migrationBuilder.InsertData(
                table: "UsersProperties",
                columns: new[]{"UserId", "Photo", "Bio", "Status", "UserRole", "LastActivity"},
                values: new object[]
                {
                    UserId,
                    null,
                    null,
                    UserProperties.ACTIVE,
                    (int)UserProperties.Role.Member,
                    DateTime.Now,
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsersProperties",
                keyColumn: "UserId",
                keyValue: UserId);
            
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: UserId);

            migrationBuilder.DeleteData(
                table: "UsersProperties",
                keyColumn: "UserId",
                keyValue: AdminId);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: AdminId);
        }
    }
}
