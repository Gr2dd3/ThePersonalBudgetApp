namespace ThePersonalBudgetApp.Core.Models;

internal class Budget
{
    public Category Income { get; set; } = new Category();
    public string? Title { get; set; }
    public string? Description { get; set; }
    public List<Category> Expenses { get; set; } = new List<Category>();
}
