using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IsekaiFantasyBE.Migrations
{
    public partial class _005preregistrationuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserRole",
                table: "UsersProperties",
                type: "int",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PreRegistrationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "VARCHAR(36)", nullable: false, collation: "ascii_general_ci"),
                    Username = table.Column<string>(type: "VARCHAR(32)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<byte[]>(type: "longblob", nullable: false),
                    EmailValidationToken = table.Column<byte[]>(type: "longblob", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegistrationUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreRegistrationUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserRole",
                table: "UsersProperties",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 1);
        }
    }
}
