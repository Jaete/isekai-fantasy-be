using IsekaiFantasyBE.Database;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IsekaiFantasyBE.Migrations
{
    public partial class _003datesdefaultvalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime?>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: DbProperties.Now);
            
            migrationBuilder.AlterColumn<DateTime?>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: DbProperties.Now);
            
            migrationBuilder.AlterColumn<DateTime?>(
                name: "LastLogin",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: DbProperties.Now);
            
            migrationBuilder.AlterColumn<DateTime?>(
                name: "LastActivity",
                table: "UsersProperties",
                type: "datetime",
                nullable: true,
                defaultValueSql: DbProperties.Now);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
