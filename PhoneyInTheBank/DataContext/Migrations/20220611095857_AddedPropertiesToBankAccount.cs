using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataContext.Migrations
{
    public partial class AddedPropertiesToBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountNumber",
                table: "BankAccounts",
                type: "int",
                maxLength: 15,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ActiveFlag",
                table: "BankAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BankruptFlag",
                table: "BankAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "BankAccounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<float>(
                name: "InvestmentAmount",
                table: "BankAccounts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "LoanAmount",
                table: "BankAccounts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "OperativeAmount",
                table: "BankAccounts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "BankAccounts",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "ActiveFlag",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BankruptFlag",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "InvestmentAmount",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "LoanAmount",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "OperativeAmount",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "BankAccounts");
        }
    }
}
