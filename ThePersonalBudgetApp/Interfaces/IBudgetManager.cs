namespace ThePersonalBudgetApp.Interfaces;

public interface IBudgetManager
{
    Task SaveBudgetAsync(Budget budget);
    Task DeleteBudgetAsync(Guid budgetId);
    Task<Budget> FetchBudgetAsync(Guid budgetId);
    Task PrintPDFAsync(Budget budget);
}
