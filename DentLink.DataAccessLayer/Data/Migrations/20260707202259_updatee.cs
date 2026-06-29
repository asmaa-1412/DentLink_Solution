using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentLink.DataAccessLayer.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IdCardUrl",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "SessionStart",
                table: "Sessions",
                newName: "SessionDate");

            migrationBuilder.RenameColumn(
                name: "SessionEnd",
                table: "Sessions",
                newName: "ActualStartTime");

            migrationBuilder.RenameColumn(
                name: "PatientArrived",
                table: "Sessions",
                newName: "IsPatientArrived");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Doctors",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "RequestedAt",
                table: "CaseRequests",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Sessions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndTime",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "University",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Faculty",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Doctors",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentIDNumber",
                table: "Doctors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualEndTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "StudentIDNumber",
                table: "Doctors");

            migrationBuilder.RenameColumn(
                name: "SessionDate",
                table: "Sessions",
                newName: "SessionStart");

            migrationBuilder.RenameColumn(
                name: "IsPatientArrived",
                table: "Sessions",
                newName: "PatientArrived");

            migrationBuilder.RenameColumn(
                name: "ActualStartTime",
                table: "Sessions",
                newName: "SessionEnd");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Doctors",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CaseRequests",
                newName: "RequestedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "University",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Faculty",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdCardUrl",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
