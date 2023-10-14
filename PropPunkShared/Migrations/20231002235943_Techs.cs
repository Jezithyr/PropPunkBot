using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPunkShared.Migrations
{
    /// <inheritdoc />
    public partial class Techs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "techs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Use = table.Column<int>(type: "integer", nullable: false),
                    Field = table.Column<int>(type: "integer", nullable: false),
                    ResearchCost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_techs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tech_requirements",
                columns: table => new
                {
                    RequiredForId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequirementsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tech_requirements", x => new { x.RequiredForId, x.RequirementsId });
                    table.ForeignKey(
                        name: "FK_tech_requirements_techs_RequiredForId",
                        column: x => x.RequiredForId,
                        principalTable: "techs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tech_requirements_techs_RequirementsId",
                        column: x => x.RequirementsId,
                        principalTable: "techs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tech_requirements_RequirementsId",
                table: "tech_requirements",
                column: "RequirementsId");

            migrationBuilder.CreateIndex(
                name: "IX_techs_Name",
                table: "techs",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tech_requirements");

            migrationBuilder.DropTable(
                name: "techs");
        }
    }
}
