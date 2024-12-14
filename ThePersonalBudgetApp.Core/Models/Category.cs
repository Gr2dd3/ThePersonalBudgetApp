namespace ThePersonalBudgetApp.Core.Models;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public float TotalAmount { get; set; } = 0;
    public List<Item> Items { get; set; } = new List<Item>();
}
