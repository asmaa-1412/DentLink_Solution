using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentLink.DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sessions");
        }
    }
}
