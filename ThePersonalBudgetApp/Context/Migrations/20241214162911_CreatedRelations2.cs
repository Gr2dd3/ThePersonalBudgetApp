using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThePersonalBudgetApp.Migrations
{
    /// <inheritdoc />
    public partial class CreatedRelations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Categories_IncomeId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_IncomeId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "IncomeId",
                table: "Budgets");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId1",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseBudgetId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IncomeBudgetId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BudgetId1",
                table: "Categories",
                column: "BudgetId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Budgets_BudgetId1",
                table: "Categories",
                column: "BudgetId1",
                principalTable: "Budgets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Budgets_BudgetId1",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BudgetId1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BudgetId1",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ExpenseBudgetId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IncomeBudgetId",
                table: "Categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "Items",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "IncomeId",
                table: "Budgets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_IncomeId",
                table: "Budgets",
                column: "IncomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Categories_IncomeId",
                table: "Budgets",
                column: "IncomeId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
