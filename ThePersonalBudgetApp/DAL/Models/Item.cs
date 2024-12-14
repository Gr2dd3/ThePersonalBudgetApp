namespace ThePersonalBudgetApp.DAL.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public float Amount { get; set; }

        public virtual Guid CategoryId { get; set; }
    }
}
