using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IsekaiFantasyBE.Migrations
{
    public partial class _006emailvalidationtokentoguid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "EmailValidationToken",
                table: "PreRegistrationUsers",
                type: "VARCHAR(36)",
                nullable: false,
                collation: "utf8mb4_bin",
                oldClrType: typeof(byte[]),
                oldType: "longblob")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Collation", "utf8mb4_bin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "EmailValidationToken",
                table: "PreRegistrationUsers",
                type: "longblob",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
