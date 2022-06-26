using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    public partial class AddedPresentAvailableToCollectOnPresent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastCollected",
                table: "Presents",
                newName: "NextPresentAvailableDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastCollectedDate",
                table: "Presents",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCollectedDate",
                table: "Presents");

            migrationBuilder.RenameColumn(
                name: "NextPresentAvailableDate",
                table: "Presents",
                newName: "LastCollected");
        }
    }
}
