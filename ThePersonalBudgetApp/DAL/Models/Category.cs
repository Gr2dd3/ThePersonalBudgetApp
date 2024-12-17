﻿namespace ThePersonalBudgetApp.DAL.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public float TotalAmount { get; set; } = 0;

        public virtual Guid IncomeBudgetId { get; set; }
        public virtual Guid ExpenseBudgetId { get; set; }
        public virtual ICollection<Item> Items { get; set; }

        public Category()
        {
            Items = new HashSet<Item>();
        }

        public void UpdateTotalAmount()
        {
            TotalAmount = Items.Sum(i => i.Amount);
        }
    }
}