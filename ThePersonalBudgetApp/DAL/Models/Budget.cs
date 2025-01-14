namespace ThePersonalBudgetApp.DAL.Models;

public class Budget
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public virtual List<Category>? Incomes { get; set; } = new List<Category>();
    public virtual List<Category>? Expenses { get; set; } = new List<Category>();

    // Start using
    public void AddCategory(Category category)
    {
        if (category.IsIncome)
            Incomes.Add(category);
        else
            Expenses.Add(category);
    }
}
