using System.ComponentModel.DataAnnotations;

namespace ThePersonalBudgetApp.DAL.Models;

public class Category
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public float TotalAmount { get; set; }
    public Guid? BudgetId { get; set; }
    public bool IsIncome { get; set; }

    [JsonIgnore]
    public virtual Budget? Budget { get; set; }
    public virtual List<Item>? Items { get; set; } = new List<Item>();

    public void UpdateTotalAmount()
    {
        if (Items != null && Items.Count > 0)
            TotalAmount = Items.Sum(i => i.Amount);
    }
}

