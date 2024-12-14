namespace ThePersonalBudgetApp.Context;



public class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }

    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }
}

