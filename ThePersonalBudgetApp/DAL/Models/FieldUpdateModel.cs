namespace ThePersonalBudgetApp.DAL.Models;


public class FieldUpdateModel
{
    public Guid CategoryId { get; set; }
    public Guid? ItemId { get; set; }
    public string FieldName { get; set; }
    public string Value { get; set; }
}
