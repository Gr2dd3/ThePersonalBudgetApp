﻿namespace ThePersonalBudgetApp.Interfaces;

public interface IBudgetManager
{
    Task SaveBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(System.Guid budgetId);
    Task<Budget> FetchBudgetAsync(System.Guid budgetId);
    Task<List<Budget>> FetchAllBudgetsAsync();
    Task DeleteBudgetCategoryOrItemAsync(System.Guid? categoryId = null, Item? item = null);

    Budget ReloadBudget(Budget budget);
    void PrintPDF(Budget budget);
    Task SaveCategoryNameAsync(Guid? categoryId, string categoryName);
    Task SaveItemNameAsync(Guid categoryId, Guid? itemId, string? itemName = null, float amount = 0);
    //Task InvokeAsync(HttpContext httpContext, IBudgetManager manager, Budget? budget = null);
}
