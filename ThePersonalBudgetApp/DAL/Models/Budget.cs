namespace ThePersonalBudgetApp.DAL.Models;

public class Budget
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public virtual List<Category>? Categories { get; set; } = new List<Category>();
}
