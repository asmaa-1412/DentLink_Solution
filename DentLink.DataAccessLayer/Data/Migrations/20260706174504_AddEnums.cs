using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentLink.DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Cases");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Cases",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "CaseRequests",
                newName: "status");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "Cases",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Typies",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "CaseRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Typies",
                table: "Cases");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Cases",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "CaseRequests",
                newName: "Status");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CaseRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
