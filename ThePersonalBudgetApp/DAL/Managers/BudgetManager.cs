namespace ThePersonalBudgetApp.DAL.Managers;

public class BudgetManager : IBudgetManager
{
    //private BudgetDbContext _context;

    public BudgetManager()
    {
        //_context = context ?? throw new ArgumentNullException(nameof(BudgetDbContext));
    }

    //public async Task InvokeAsync(HttpContext httpContext, IBudgetManager manager, Budget? budget = null)
    //{
    //    if (budget != null)
    //    {
    //        await manager.SaveBudgetAsync(budget);
    //    }
    //    await _next(httpContext);
    //}
    // TODO: Se till att existerande budget sparar ordentligt. 
    public async Task SaveBudgetAsync(Budget budget)
    {
        if (budget == null)
            throw new ArgumentNullException(nameof(budget), "Budget cannot be null");

        if (budget.Id == Guid.Empty)
            return;

        using (var context = new BudgetDbContext())
        {
            try
            {
                // Try fetching only id from db?
                Budget? existingBudget = await GetBudgetByIdAsync(budget.Id);

                if (existingBudget != null && existingBudget.Id != Guid.Empty)
                {
                    await UpdateBudgetAsync(budget, existingBudget);
                }
                else
                {
                    await AddNewBudgetAsync(budget);
                }
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Operation canceled. Det kan bero på en timeout eller annullerad token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the budget.", ex);
            }
        }
    }




    public async Task DeleteBudgetAsync(System.Guid budgetId)
    {
        //var budget = await _context.Budgets.FindAsync(budgetId);
        var budget = await FetchBudgetAsync(budgetId);
        if (budget == null)
        {
            throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");
        }
        using (var context = new BudgetDbContext())
        {
            context.Budgets.Remove(budget);
            await context.SaveChangesAsync();

        }

    }


    //public async Task DeleteBudgetCategoryOrItemAsync(Guid? categoryId = null, Item? item = null)
    //{
    //    if (categoryId == null && item == null)
    //    {
    //        throw new ArgumentException("At least one parameter must be provided.");
    //    }

    //    if (categoryId is not null)
    //    {
    //        Category? category = null;
    //        category = await _context.Categories
    //            .Include(c => c.Items)
    //            .FirstOrDefaultAsync(c => c.Id == categoryId);

    //        if (category is null)
    //        {
    //            throw new NullReferenceException("Category not found.");
    //        }
    //        else
    //        {
    //            if (category.Items is not null || category.Items?.Count == 0)
    //            {
    //                _context.Items.RemoveRange(category.Items);
    //            }
    //            _context.Categories.Remove(category);
    //        }
    //    }

    //    if (item is not null)
    //    {
    //        _context.Remove(item);
    //    }

    //    await _context.SaveChangesAsync();
    //}

    //public async Task<Budget> FetchBudgetAsync(Guid budgetId)
    //{
    //    var fetchedBudget = await _context.Budgets
    //        .Include(b => b.Categories)
    //            .ThenInclude(c => c.Items)
    //        .FirstOrDefaultAsync(b => b.Id == budgetId);

    //    if (fetchedBudget == null)
    //    {
    //        throw new KeyNotFoundException($"Budget with ID {budgetId} not found.");
    //    }

    //    return fetchedBudget;
    //}

    //public async Task<List<Budget>> FetchAllBudgetsAsync()
    //{
    //    var budgets = await _context.Budgets!
    //        .Include(b => b.Categories!)
    //            .ThenInclude(i => i.Items)
    //        .ToListAsync();

    //    if (budgets == null || !budgets.Any())
    //    {
    //        throw new Exception("No budgets found.");
    //    }
    //    return budgets;
    //}

    //public Budget ReloadBudget(Budget budget)
    //{
    //    _context.Entry(budget).Reload();
    //    return budget;
    //}

    //public void PrintPDF(Budget budget)
    //{
    //    List<Category> incomes = new List<Category>();
    //    List<Category> expenses = new List<Category>();
    //    if (budget == null)
    //        throw new ArgumentNullException(nameof(budget), "Budget cannot be null");

    //    incomes = budget.Categories!.Where(c => c.IsIncome).ToList();
    //    expenses = budget.Categories!.Where(c => !c.IsIncome).ToList();


    //    var document = Document.Create(container =>
    //    {
    //        container.Page(page =>
    //        {
    //            page.Size(PageSizes.A4);
    //            page.Margin(1, Unit.Centimetre);
    //            page.Content().Column(column =>
    //            {
    //                column.Item().Text($"Budget Title: {budget.Title}").FontSize(20).Bold();
    //                column.Item().Text($"Description: {budget.Description}").FontSize(14);

    //                column.Item().Text("Incomes:").FontSize(16).Bold();
    //                foreach (var income in incomes)
    //                {
    //                    column.Item().Text($"- {income.Name}");
    //                    foreach (var item in income.Items!)
    //                    {
    //                        column.Item().Text($"  * {item.Name}: {item.Amount:C}");
    //                    }
    //                }

    //                column.Item().Text("Expenses:").FontSize(16).Bold();
    //                foreach (var expense in expenses)
    //                {
    //                    column.Item().Text($"- {expense.Name}");
    //                    foreach (var item in expense.Items!)
    //                    {
    //                        column.Item().Text($"  * {item.Name}: {item.Amount:C}");
    //                    }
    //                }
    //            });
    //        });
    //    });

    //    var pdfFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Budget.pdf");
    //    document.GeneratePdf(pdfFilePath);
    //    // Alternativt: Returnera PDF som en ström för nedladdning i en webbapplikation
    //    //return File(pdfStream, "application/pdf", "Budget.pdf");

    //}



    //#region Private Methods

    //#region Save Methods

    //private async Task<Budget?> GetBudgetByIdAsync(Guid budgetId)
    //{
    //    try
    //    {
    //        return await _context.Budgets!
    //            .FirstOrDefaultAsync(b => b.Id == budgetId);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error querying database: {ex.Message}");
    //        throw;
    //    }
    //}

    //private async Task AddNewBudgetAsync(Budget newBudget)
    //{
    //    var transaction = await _context.Database.BeginTransactionAsync();
    //    try
    //    {
    //        await _context.Budgets.AddAsync(newBudget);
    //        await _context.SaveChangesAsync();

    //        await SaveCategoriesWithItemsAsync(newBudget.Categories!, newBudget.Id);

    //        await transaction.CommitAsync();
    //    }
    //    catch
    //    {
    //        await transaction.RollbackAsync();
    //        throw;
    //    }
    //}

    //private async Task UpdateBudgetAsync(Budget updatedBudget, Budget existingBudget)
    //{
    //    using (var transaction = await _context.Database.BeginTransactionAsync())
    //    {
    //        try
    //        {
    //            _context.Entry(existingBudget).CurrentValues.SetValues(updatedBudget);

    //            await UpdateCategoriesWithItemsAsync(updatedBudget.Categories!, existingBudget.Categories!, updatedBudget.Id);

    //            await _context.SaveChangesAsync();
    //            await transaction.CommitAsync();
    //        }
    //        catch
    //        {
    //            await transaction.RollbackAsync();
    //            throw;
    //        }
    //    }
    //}

    //private async Task SaveCategoriesWithItemsAsync(IEnumerable<Category> categories, Guid budgetId)
    //{
    //    foreach (var category in categories)
    //    {
    //        category.BudgetId = budgetId;
    //        await _context.Categories.AddAsync(category);

    //        foreach (var item in category.Items!)
    //        {
    //            item.CategoryId = category.Id;
    //            await _context.Items.AddAsync(item);
    //        }
    //    }
    //}

    //private async Task UpdateCategoriesWithItemsAsync(IEnumerable<Category> updatedCategories, IEnumerable<Category> existingCategories, Guid budgetId)
    //{
    //    foreach (var existingCategory in existingCategories)
    //    {
    //        if (!updatedCategories.Any(c => c.Id == existingCategory.Id))
    //        {
    //            _context.Categories.Remove(existingCategory);
    //        }
    //    }

    //    foreach (var updatedCategory in updatedCategories)
    //    {
    //        var existingCategory = existingCategories.FirstOrDefault(c => c.Id == updatedCategory.Id);

    //        if (existingCategory != null)
    //        {
    //            await UpdateExistingCategoryWithItemsAsync(updatedCategory, existingCategory);
    //        }
    //        else
    //        {
    //            updatedCategory.BudgetId = budgetId;
    //            await AddNewCategoryWithItemsAsync(updatedCategory);
    //        }
    //    }
    //}

    //private async Task UpdateExistingCategoryWithItemsAsync(Category updatedCategory, Category existingCategory)
    //{
    //    _context.Entry(existingCategory).CurrentValues.SetValues(updatedCategory);

    //    foreach (var updatedItem in updatedCategory.Items!)
    //    {
    //        var existingItem = existingCategory.Items!.FirstOrDefault(i => i.Id == updatedItem.Id);

    //        if (existingItem != null)
    //        {
    //            _context.Entry(existingItem).CurrentValues.SetValues(updatedItem);
    //        }
    //        else
    //        {
    //            updatedItem.CategoryId = updatedCategory.Id;
    //            await _context.Items.AddAsync(updatedItem);
    //        }
    //    }

    //    foreach (var existingItem in existingCategory.Items!.ToList())
    //    {
    //        if (!updatedCategory.Items!.Any(i => i.Id == existingItem.Id))
    //        {
    //            _context.Items.Remove(existingItem);
    //        }
    //    }
    //}

    //private async Task AddNewCategoryWithItemsAsync(Category newCategory)
    //{
    //    await _context.Categories.AddAsync(newCategory);

    //    foreach (var item in newCategory.Items!)
    //    {
    //        item.CategoryId = newCategory.Id;
    //        await _context.Items.AddAsync(item);
    //    }
    //}

    //#endregion

    ////private async Task TestingDbConnectionAsync()
    ////{
    ////    try
    ////    {
    ////        var canConnect = await _context.Database.CanConnectAsync();
    ////        if (!canConnect)
    ////        {
    ////            Console.WriteLine("Connection failed.");
    ////        }
    ////        else
    ////        {
    ////            Console.WriteLine("Connection is open.");
    ////        }
    ////    }
    ////    catch (Exception ex)
    ////    {
    ////        Console.WriteLine($"Error while checking database connection: {ex.Message}");
    ////    }
    ////}


    #endregion
}
