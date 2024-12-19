namespace ThePersonalBudgetApp.DAL.Models;

public class Budget
{
    public Guid Id { get; set; }
    public virtual List<Category> Incomes { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public virtual List<Category> Expenses { get; set; }

    public Budget()
    {
        Expenses = new List<Category>();
        Incomes = new List<Category>();
    }
}
