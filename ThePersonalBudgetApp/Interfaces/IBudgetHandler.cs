namespace ThePersonalBudgetApp.Interfaces;

public interface IBudgetHandler
{
    Budget CurrentBudget { get; set; }
    Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null);
}
