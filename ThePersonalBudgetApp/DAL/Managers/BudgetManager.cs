namespace ThePersonalBudgetApp.DAL.Managers;

public class BudgetManager : IBudgetManager
{
    BudgetDbContext _context;
    public BudgetManager(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task SaveBudgetAsync(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget), "Budget cannot be null");

        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingBudget = await _context.Budgets.FindAsync(budget.Id);
                    if (existingBudget != null)
                        _context.Budgets.Update(budget);
                    else
                        await _context.Budgets.AddAsync(budget);

                    await _context.SaveChangesAsync();

                    var budgetId = budget.Id;

                    foreach (var category in budget.Incomes.Concat(budget.Expenses))
                    {
                        category.BudgetId = budgetId;

                        var existingCategory = await _context.Categories
                            .FirstOrDefaultAsync(c => c.Id == category.Id);
                        if (existingCategory != null)
                        {
                            _context.Categories.Update(category);
                        }
                        else
                        {
                            await _context.Categories.AddAsync(category);
                        }

                        foreach (var item in category.Items)
                        {
                            item.CategoryId = category.Id;

                            var existingItem = await _context.Items
                                .FirstOrDefaultAsync(i => i.Id == item.Id);
                            if (existingItem != null)
                            {
                                _context.Items.Update(item);
                            }
                            else
                            {
                                await _context.Items.AddAsync(item);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("An error occurred while saving the budget and its related data.", ex);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving the budget.", ex);
        }
    }



    public async Task DeleteBudgetAsync(System.Guid budgetId)
    {
        var budget = await _context.Budgets.FindAsync(budgetId);
        if (budget == null)
        {
            throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
    }

    public async Task<Budget> FetchBudgetAsync(Guid budgetId)
    {
        var budget = await _context.Budgets
            .Include(b => b.Incomes)
                .ThenInclude(c => c.Items)
            .Include(b => b.Expenses)
                .ThenInclude(c => c.Items)
            .FirstOrDefaultAsync(b => b.Id == budgetId);

        if (budget == null)
        {
            throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");
        }

        return budget;
    }


    public async Task<List<Budget>> FetchAllBudgetsAsync()
    {
        var budgets = await _context.Budgets
            .Include(b => b.Incomes)
                .ThenInclude(i => i.Items)
            .Include(b => b.Expenses)
                .ThenInclude(e => e.Items)
            .ToListAsync();

        if (budgets == null || !budgets.Any())
        {
            throw new Exception("No budgets found.");
        }
        return budgets;
    }

    public void PrintPDF(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget), "Budget cannot be null");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.Content().Column(column =>
                {
                    column.Item().Text($"Budget Title: {budget.Title}").FontSize(20).Bold();
                    column.Item().Text($"Description: {budget.Description}").FontSize(14);

                    column.Item().Text("Incomes:").FontSize(16).Bold();
                    foreach (var income in budget.Incomes)
                    {
                        column.Item().Text($"- {income.Name}");
                        foreach (var item in income.Items)
                        {
                            column.Item().Text($"  * {item.Name}: {item.Amount:C}");
                        }
                    }

                    column.Item().Text("Expenses:").FontSize(16).Bold();
                    foreach (var expense in budget.Expenses)
                    {
                        column.Item().Text($"- {expense.Name}");
                        foreach (var item in expense.Items)
                        {
                            column.Item().Text($"  * {item.Name}: {item.Amount:C}");
                        }
                    }
                });
            });
        });

        var pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Budget.pdf");
        document.GeneratePdf(pdfFilePath);
        // Alternativt: Returnera PDF som en ström för nedladdning i en webbapplikation
        //return File(pdfStream, "application/pdf", "Budget.pdf");

    }

}
