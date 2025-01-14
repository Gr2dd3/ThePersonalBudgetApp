using ThePersonalBudgetApp.DAL.Models;

namespace ThePersonalBudgetApp.DAL.Managers;

public class BudgetManager : IBudgetManager
{
    BudgetDbContext _context;
    public BudgetManager(BudgetDbContext context)
    {
        _context = context;
    }
    // TODO: Se till att existerande budget sparar ordentligt. 
    public async Task SaveBudgetAsync(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget), "Budget cannot be null");

        if (budget.Id == Guid.Empty)
            return;
        //TODO: Delete budget from db and then make a sync - Add-Migration UpdatedOnModeling, Update-Database
        try
        {
            var existingBudget = await _context.Budgets
                .Include(b => b.Incomes)
                    .ThenInclude(c => c.Items)
                .Include(b => b.Expenses)
                    .ThenInclude(c => c.Items)
                .FirstOrDefaultAsync(b => b.Id == budget.Id);

            if (existingBudget != null)
            {
                await UpdateBudgetAsync(budget, existingBudget);
            }
            else
            {
                await AddNewBudgetAsync(budget);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while saving the budget.", ex);
        }
    }
    #region private methods for SaveBudgetAsync

    private async Task AddNewBudgetAsync(Budget budget)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                await _context.Budgets.AddAsync(budget);
                await _context.SaveChangesAsync();

                var budgetId = budget.Id;

                await SaveCategoriesAndItemsAsync(budget.Incomes, budgetId);

                await SaveCategoriesAndItemsAsync(budget.Expenses, budgetId);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    private async Task UpdateBudgetAsync(Budget budget, Budget existingBudget)
    {
        _context.Entry(existingBudget).CurrentValues.SetValues(budget);

        await UpdateCategoriesAndItemsAsync(budget.Incomes, existingBudget.Incomes, budget.Id);

        await UpdateCategoriesAndItemsAsync(budget.Expenses, existingBudget.Expenses, budget.Id);

        await _context.SaveChangesAsync();
    }

    private async Task SaveCategoriesAndItemsAsync(IEnumerable<Category> categories, Guid budgetId)
    {
        foreach (var category in categories)
        {
            category.BudgetId = budgetId;

            await _context.Categories.AddAsync(category);

            foreach (var item in category.Items)
            {
                item.CategoryId = category.Id;
                await _context.Items.AddAsync(item);
            }
        }
    }

    private async Task UpdateCategoriesAndItemsAsync(IEnumerable<Category> newCategories, IEnumerable<Category> existingCategories, Guid budgetId)
    {
        foreach (var existingCategory in existingCategories)
        {
            if (!newCategories.Any(c => c.Id == existingCategory.Id))
            {
                _context.Categories.Remove(existingCategory);
            }
        }

        foreach (var newCategory in newCategories)
        {
            var existingCategory = existingCategories.FirstOrDefault(c => c.Id == newCategory.Id);

            if (existingCategory != null)
            {
                _context.Entry(existingCategory).CurrentValues.SetValues(newCategory);

                foreach (var item in newCategory.Items)
                {
                    var existingItem = existingCategory.Items.FirstOrDefault(i => i.Id == item.Id);
                    if (existingItem != null)
                    {
                        _context.Entry(existingItem).CurrentValues.SetValues(item);
                    }
                    else
                    {
                        item.CategoryId = newCategory.Id;
                        await _context.Items.AddAsync(item);
                    }
                }
            }
            else
            {
                if (existingCategory != null)
                {
                    newCategory.BudgetId = existingCategory.BudgetId;
                }
                else
                {
                    newCategory.BudgetId = budgetId;
                }

                await _context.Categories.AddAsync(newCategory);

                foreach (var item in newCategory.Items)
                {
                    item.CategoryId = newCategory.Id;
                    await _context.Items.AddAsync(item);
                }
            }
        }
    }

    #endregion

    public async Task DeleteBudgetAsync(System.Guid budgetId)
    {
        //var budget = await _context.Budgets.FindAsync(budgetId);
        var budget = await FetchBudgetAsync(budgetId);
        if (budget == null)
        {
            throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteBudgetCategoryOrItemAsync(Guid? categoryId = null, Item? item = null)
    {
        if (categoryId == null && item == null)
        {
            throw new ArgumentException("At least one parameter must be provided.");
        }

        if (categoryId is not null)
        {
            Category? category = null;
            category = await _context.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                throw new NullReferenceException("Category not found.");
            }
            else
            {
                if (category.Items is not null || category.Items?.Count == 0)
                {
                    _context.Items.RemoveRange(category.Items);
                }
                _context.Categories.Remove(category);
            }
        }

        if (item is not null)
        {
            _context.Remove(item);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Budget> FetchBudgetAsync(Guid budgetId)
    {
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        var budget = await _context.Budgets
            .Include(b => b.Incomes)
                .ThenInclude(c => c.Items)
            .Include(b => b.Expenses)
                .ThenInclude(c => c.Items)
            .FirstOrDefaultAsync(b => b.Id == budgetId);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

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

    public Budget ReloadBudget(Budget budget)
    {
        _context.Entry(budget).Reload();
        return budget;
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
