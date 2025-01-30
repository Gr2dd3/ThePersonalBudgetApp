namespace ThePersonalBudgetApp.DAL.Models;


public class FieldUpdateModel
{
    public string? CategoryId { get; set; }
    public string? ItemId { get; set; }
    public string? FieldName { get; set; }
    public string? Value { get; set; }

    public DateTime? Timestamp { get; set; }
}
