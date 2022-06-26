using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    public partial class ModifiedTransactionHistoryTransactionTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionHistories_TransactionTypes_TransactionTypeId",
                table: "TransactionHistories");

            migrationBuilder.DropIndex(
                name: "IX_TransactionHistories_TransactionTypeId",
                table: "TransactionHistories");

            migrationBuilder.DropColumn(
                name: "TransactionTypeId",
                table: "TransactionHistories");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "TransactionHistories",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "TransactionHistories");

            migrationBuilder.AddColumn<int>(
                name: "TransactionTypeId",
                table: "TransactionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionHistories_TransactionTypeId",
                table: "TransactionHistories",
                column: "TransactionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionHistories_TransactionTypes_TransactionTypeId",
                table: "TransactionHistories",
                column: "TransactionTypeId",
                principalTable: "TransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
