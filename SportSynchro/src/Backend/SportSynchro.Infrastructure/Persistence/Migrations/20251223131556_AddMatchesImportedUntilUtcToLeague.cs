using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportSynchro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchesImportedUntilUtcToLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MatchesImportedUntilUtc",
                table: "Leagues",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchesImportedUntilUtc",
                table: "Leagues");
        }
    }
}
