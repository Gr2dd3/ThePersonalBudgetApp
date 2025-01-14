namespace ThePersonalBudgetApp.Context;

public class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }

    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Budget)
            .WithMany(b => b.Incomes)
            .HasForeignKey(c => c.BudgetId)
            .HasPrincipalKey(b => b.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Budget)
            .WithMany(b => b.Expenses)
            .HasForeignKey(c => c.BudgetId)
            .HasPrincipalKey(b => b.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Budget>()
            .Navigation(b => b.Incomes)
            .HasField("_incomes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<Budget>()
            .Navigation(b => b.Expenses)
            .HasField("_expenses")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

}
