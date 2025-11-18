using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleRDBMSRestfulAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "connection_infos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    db_type = table.Column<string>(type: "text", nullable: false),
                    connection_string = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<long>(type: "int8", nullable: false, defaultValueSql: "FLOOR(EXTRACT(EPOCH FROM NOW()) * 1000)"),
                    updated_at = table.Column<long>(type: "int8", nullable: false, defaultValueSql: "FLOOR(EXTRACT(EPOCH FROM NOW()) * 1000)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_connection_infos", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "connection_infos");
        }
    }
}
