namespace ThePersonalBudgetApp.DAL.Models;

public class Budget
{
    public Guid Id { get; set; }
    public virtual ICollection<Category> Incomes { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<Category> Expenses { get; set; }

    public Budget()
    {
        Expenses = new HashSet<Category>();
        Incomes = new HashSet<Category>();
    }
}
