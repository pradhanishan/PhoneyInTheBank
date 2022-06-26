using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    public partial class AddedActiveFlagInInvestments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investment_AspNetUsers_ApplicationUserId",
                table: "Investment");

            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Organizations_OrganizationId",
                table: "Investment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Investment",
                table: "Investment");

            migrationBuilder.RenameTable(
                name: "Investment",
                newName: "Investments");

            migrationBuilder.RenameIndex(
                name: "IX_Investment_OrganizationId",
                table: "Investments",
                newName: "IX_Investments_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Investment_ApplicationUserId",
                table: "Investments",
                newName: "IX_Investments_ApplicationUserId");

            migrationBuilder.AddColumn<bool>(
                name: "ActiveFlag",
                table: "Investments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Investments",
                table: "Investments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_AspNetUsers_ApplicationUserId",
                table: "Investments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Investments_Organizations_OrganizationId",
                table: "Investments",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investments_AspNetUsers_ApplicationUserId",
                table: "Investments");

            migrationBuilder.DropForeignKey(
                name: "FK_Investments_Organizations_OrganizationId",
                table: "Investments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Investments",
                table: "Investments");

            migrationBuilder.DropColumn(
                name: "ActiveFlag",
                table: "Investments");

            migrationBuilder.RenameTable(
                name: "Investments",
                newName: "Investment");

            migrationBuilder.RenameIndex(
                name: "IX_Investments_OrganizationId",
                table: "Investment",
                newName: "IX_Investment_OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Investments_ApplicationUserId",
                table: "Investment",
                newName: "IX_Investment_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Investment",
                table: "Investment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_AspNetUsers_ApplicationUserId",
                table: "Investment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Organizations_OrganizationId",
                table: "Investment",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
