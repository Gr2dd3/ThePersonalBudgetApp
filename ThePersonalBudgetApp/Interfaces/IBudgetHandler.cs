namespace ThePersonalBudgetApp.Interfaces;
public interface IBudgetHandler
{
    Budget CurrentBudget { get; set; }
    Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null);
    Task<IActionResult> OnPostAddCategoryAsync(string categoryType);
    Task<IActionResult> OnPostRemoveCategoryAsync(Guid categoryId, string? categoryName = null);
    Task<IActionResult> OnPostAddItemAsync(Guid categoryId);
    Task<IActionResult> OnPostRemoveItemAsync(Guid categoryId, int itemIndex);
}
