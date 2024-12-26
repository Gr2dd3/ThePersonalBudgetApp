namespace ThePersonalBudgetApp.Interfaces;

public interface IBudgetManager
{
    Task SaveBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(System.Guid budgetId);
    Task<Budget> FetchBudgetAsync(System.Guid budgetId);
    Task<List<Budget>> FetchAllBudgetsAsync();
    void PrintPDF(Budget budget);
}
