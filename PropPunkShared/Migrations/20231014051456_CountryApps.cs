using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PropPunkShared.Migrations
{
    /// <inheritdoc />
    public partial class CountryApps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "applications_country",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ShortName = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    NationalLanguages = table.Column<string>(type: "text", nullable: false),
                    CapitalCityDescription = table.Column<string>(type: "text", nullable: false),
                    PopulationBreakdown = table.Column<string>(type: "text", nullable: false),
                    GovernmentDescription = table.Column<string>(type: "text", nullable: false),
                    EconomicDescription = table.Column<string>(type: "text", nullable: false),
                    Centralization = table.Column<string>(type: "text", nullable: false),
                    Cohesion = table.Column<string>(type: "text", nullable: false),
                    TerrainClimate = table.Column<string>(type: "text", nullable: false),
                    MajorCities = table.Column<string>(type: "text", nullable: false),
                    MajorPorts = table.Column<string>(type: "text", nullable: false),
                    Resources = table.Column<string>(type: "text", nullable: false),
                    EconomicBoons = table.Column<string>(type: "text", nullable: false),
                    Struggles = table.Column<string>(type: "text", nullable: false),
                    TechLevel = table.Column<string>(type: "text", nullable: false),
                    EducationLevel = table.Column<string>(type: "text", nullable: false),
                    MilitaryDescription = table.Column<string>(type: "text", nullable: false),
                    MilitaryStruggles = table.Column<string>(type: "text", nullable: false),
                    SocialServices = table.Column<string>(type: "text", nullable: false),
                    HistoricalCulture = table.Column<string>(type: "text", nullable: false),
                    ModernCulture = table.Column<string>(type: "text", nullable: false),
                    Religion = table.Column<string>(type: "text", nullable: false),
                    CivilStrife = table.Column<string>(type: "text", nullable: false),
                    ForeignRelations = table.Column<string>(type: "text", nullable: false),
                    BorderStates = table.Column<string>(type: "text", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: false),
                    Flag = table.Column<string>(type: "text", nullable: false),
                    Roundel = table.Column<string>(type: "text", nullable: false),
                    BattleFlag = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications_country", x => x.Id);
                    table.ForeignKey(
                        name: "FK_applications_country_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_applications_country_Name",
                table: "applications_country",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_country_ShortName",
                table: "applications_country",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_applications_country_UserId",
                table: "applications_country",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "applications_country");
        }
    }
}
