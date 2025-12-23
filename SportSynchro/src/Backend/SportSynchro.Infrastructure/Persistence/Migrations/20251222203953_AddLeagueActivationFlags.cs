using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSynchro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueActivationFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MatchesImported",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TeamsImported",
                table: "Leagues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchesImported",
                table: "Leagues");

            migrationBuilder.DropColumn(
                name: "TeamsImported",
                table: "Leagues");
        }
    }
}
