using IsekaiFantasyBE.Models.Users;
using IsekaiFantasyBE.Services.Utils;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IsekaiFantasyBE.Migrations
{
    public partial class _003usertobebanned : Migration
    {
        private static readonly Guid BannedId = Guid.Parse("C0C0C0C0-C0C0-C0C0-C0C0-C0C0C0C0C0C0");
        private static readonly string BannedPass = "EDNALDOpereiraBANIDO123!!";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]{"Id", "Username", "Email", "Password", "CreatedAt", "UpdatedAt", "LastLogin"},
                values: new object[]
                {
                    BannedId,
                    "BaNiDo",
                    "banned@user.com",
                    PasswordService.Encrypt(BannedPass),
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
                    BannedId,
                    null,
                    null,
                    UserProperties.ACTIVE,
                    (int)UserProperties.Role.Admin,
                    DateTime.Now,
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "BannedUsers");
            
            migrationBuilder.DeleteData(
                table: "UsersProperties",
                keyColumn: "UserId",
                keyValue: BannedId);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: BannedId);
        }
    }
}
