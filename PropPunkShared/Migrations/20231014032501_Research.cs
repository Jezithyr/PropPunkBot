using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPunkShared.Migrations
{
    /// <inheritdoc />
    public partial class Research : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_countries_governments_GovernmentModelId",
                table: "countries");

            migrationBuilder.DropForeignKey(
                name: "FK_tech_requirements_techs_RequiredForId",
                table: "tech_requirements");

            migrationBuilder.DropForeignKey(
                name: "FK_tech_requirements_techs_RequirementsId",
                table: "tech_requirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tech_requirements",
                table: "tech_requirements");

            migrationBuilder.RenameTable(
                name: "tech_requirements",
                newName: "TechnologyModelTechnologyModel");

            migrationBuilder.RenameColumn(
                name: "ResearchCost",
                table: "techs",
                newName: "RawResearchCost");

            migrationBuilder.RenameColumn(
                name: "GovernmentModelId",
                table: "countries",
                newName: "ResearchId");

            migrationBuilder.RenameIndex(
                name: "IX_countries_GovernmentModelId",
                table: "countries",
                newName: "IX_countries_ResearchId");

            migrationBuilder.RenameIndex(
                name: "IX_tech_requirements_RequirementsId",
                table: "TechnologyModelTechnologyModel",
                newName: "IX_TechnologyModelTechnologyModel_RequirementsId");

            migrationBuilder.AddColumn<Guid>(
                name: "GovernmentId",
                table: "countries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TechnologyModelTechnologyModel",
                table: "TechnologyModelTechnologyModel",
                columns: new[] { "RequiredForId", "RequirementsId" });

            migrationBuilder.CreateTable(
                name: "countries_research",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PointOverflow = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries_research", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_countries_research_countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "countries_research_mods",
                columns: table => new
                {
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Field = table.Column<int>(type: "integer", nullable: false),
                    Modifier = table.Column<float>(type: "real", nullable: false),
                    CountryResearchCountryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries_research_mods", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_countries_research_mods_countries_research_CountryResearchC~",
                        column: x => x.CountryResearchCountryId,
                        principalTable: "countries_research",
                        principalColumn: "CountryId");
                });

            migrationBuilder.CreateTable(
                name: "countries_research_slots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PointProgress = table.Column<int>(type: "integer", nullable: false),
                    CountryId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechId = table.Column<Guid>(type: "uuid", nullable: false),
                    CountryResearchCountryId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries_research_slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_countries_research_slots_countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_countries_research_slots_countries_research_CountryResearch~",
                        column: x => x.CountryResearchCountryId,
                        principalTable: "countries_research",
                        principalColumn: "CountryId");
                    table.ForeignKey(
                        name: "FK_countries_research_slots_techs_TechId",
                        column: x => x.TechId,
                        principalTable: "techs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_countries_GovernmentId",
                table: "countries",
                column: "GovernmentId");

            migrationBuilder.CreateIndex(
                name: "IX_countries_research_mods_CountryId_Field",
                table: "countries_research_mods",
                columns: new[] { "CountryId", "Field" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_research_mods_CountryResearchCountryId",
                table: "countries_research_mods",
                column: "CountryResearchCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_countries_research_slots_CountryId",
                table: "countries_research_slots",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_countries_research_slots_CountryResearchCountryId",
                table: "countries_research_slots",
                column: "CountryResearchCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_countries_research_slots_TechId",
                table: "countries_research_slots",
                column: "TechId");

            migrationBuilder.AddForeignKey(
                name: "FK_countries_countries_research_ResearchId",
                table: "countries",
                column: "ResearchId",
                principalTable: "countries_research",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_countries_governments_GovernmentId",
                table: "countries",
                column: "GovernmentId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnologyModelTechnologyModel_techs_RequiredForId",
                table: "TechnologyModelTechnologyModel",
                column: "RequiredForId",
                principalTable: "techs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TechnologyModelTechnologyModel_techs_RequirementsId",
                table: "TechnologyModelTechnologyModel",
                column: "RequirementsId",
                principalTable: "techs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_countries_countries_research_ResearchId",
                table: "countries");

            migrationBuilder.DropForeignKey(
                name: "FK_countries_governments_GovernmentId",
                table: "countries");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnologyModelTechnologyModel_techs_RequiredForId",
                table: "TechnologyModelTechnologyModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TechnologyModelTechnologyModel_techs_RequirementsId",
                table: "TechnologyModelTechnologyModel");

            migrationBuilder.DropTable(
                name: "countries_research_mods");

            migrationBuilder.DropTable(
                name: "countries_research_slots");

            migrationBuilder.DropTable(
                name: "countries_research");

            migrationBuilder.DropIndex(
                name: "IX_countries_GovernmentId",
                table: "countries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TechnologyModelTechnologyModel",
                table: "TechnologyModelTechnologyModel");

            migrationBuilder.DropColumn(
                name: "GovernmentId",
                table: "countries");

            migrationBuilder.RenameTable(
                name: "TechnologyModelTechnologyModel",
                newName: "tech_requirements");

            migrationBuilder.RenameColumn(
                name: "RawResearchCost",
                table: "techs",
                newName: "ResearchCost");

            migrationBuilder.RenameColumn(
                name: "ResearchId",
                table: "countries",
                newName: "GovernmentModelId");

            migrationBuilder.RenameIndex(
                name: "IX_countries_ResearchId",
                table: "countries",
                newName: "IX_countries_GovernmentModelId");

            migrationBuilder.RenameIndex(
                name: "IX_TechnologyModelTechnologyModel_RequirementsId",
                table: "tech_requirements",
                newName: "IX_tech_requirements_RequirementsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tech_requirements",
                table: "tech_requirements",
                columns: new[] { "RequiredForId", "RequirementsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_countries_governments_GovernmentModelId",
                table: "countries",
                column: "GovernmentModelId",
                principalTable: "governments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tech_requirements_techs_RequiredForId",
                table: "tech_requirements",
                column: "RequiredForId",
                principalTable: "techs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tech_requirements_techs_RequirementsId",
                table: "tech_requirements",
                column: "RequirementsId",
                principalTable: "techs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
