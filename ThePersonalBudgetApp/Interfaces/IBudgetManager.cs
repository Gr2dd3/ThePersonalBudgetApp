namespace ThePersonalBudgetApp.Interfaces;

public interface IBudgetManager
{
    Task SaveBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(Guid budgetId);
    Task<Budget> FetchBudgetAsync(Guid budgetId);
    Task<List<Budget>> FetchAllBudgetsAsync();
    void PrintPDF(Budget budget);
}
