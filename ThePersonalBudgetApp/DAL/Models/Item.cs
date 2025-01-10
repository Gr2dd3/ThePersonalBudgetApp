namespace ThePersonalBudgetApp.DAL.Models;

public class Item
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public float Amount { get; set; }
    public Guid? CategoryId { get; set; }
    [JsonIgnore]
    public virtual Category? Category { get; set; }
}

