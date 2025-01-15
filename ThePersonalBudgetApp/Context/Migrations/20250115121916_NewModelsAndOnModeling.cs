using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThePersonalBudgetApp.Migrations
{
    /// <inheritdoc />
    public partial class NewModelsAndOnModeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Budgets_BudgetId",
                table: "Categories");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Budgets_BudgetId",
                table: "Categories",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Categories_Budgets_BudgetId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.AddColumn<Guid>(
                name: "BudgetId1",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BudgetId1",
                table: "Categories",
                column: "BudgetId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Budgets_BudgetId",
                table: "Categories",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id");

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
                principalColumn: "Id");
        }
    }
}
