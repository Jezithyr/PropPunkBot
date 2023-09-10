using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPunkShared.Migrations
{
    /// <inheritdoc />
    public partial class Governments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GovernmentModelId",
                table: "countries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "governments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_governments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_countries_GovernmentModelId",
                table: "countries",
                column: "GovernmentModelId");

            migrationBuilder.CreateIndex(
                name: "IX_governments_Name",
                table: "governments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_countries_governments_GovernmentModelId",
                table: "countries",
                column: "GovernmentModelId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_countries_governments_GovernmentModelId",
                table: "countries");

            migrationBuilder.DropTable(
                name: "governments");

            migrationBuilder.DropIndex(
                name: "IX_countries_GovernmentModelId",
                table: "countries");

            migrationBuilder.DropColumn(
                name: "GovernmentModelId",
                table: "countries");
        }
    }
}
