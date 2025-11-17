using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFrenchGradingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GradeComments",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "Enrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GradedBy",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GradedByFacultyId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LetterGrade",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_GradedByFacultyId",
                table: "Enrollments",
                column: "GradedByFacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_GradedByFacultyId",
                table: "Enrollments",
                column: "GradedByFacultyId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_GradedByFacultyId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_GradedByFacultyId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "GradeComments",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "GradedBy",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "GradedByFacultyId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "LetterGrade",
                table: "Enrollments");
        }
    }
}
