using Microsoft.EntityFrameworkCore;
using ThePersonalBudgetApp.DAL.Models;

namespace ThePersonalBudgetApp.Pages;

public class WorkOnBudgetModel : PageModel
{
    private IBudgetManager _iBudgetManager;
    public WorkOnBudgetModel(IBudgetManager budgetManager)
    {
        if (_iBudgetManager == null)
            _iBudgetManager = budgetManager;
    }

    public bool IsWorkingOnBudget { get; set; }
    public List<Budget>? Budgets { get; set; }

    [BindProperty]
    public Budget? SelectedBudget { get; set; }

    public async Task OnGetAsync()
    {
        Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
    }

    public async Task OnPostAsync()
    {
        if (Request.Form["action"] == "back")
        {
            IsWorkingOnBudget = false;
            SelectedBudget = null;
        }
        else if (Guid.TryParse(Request.Form["selectedBudgetId"], out Guid budgetId))
        {
            SelectedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
            HttpContext.Session.Set("SelectedBudgetId", SelectedBudget.Id.ToByteArray());
            IsWorkingOnBudget = true;
        }
        else
        {
            throw new Exception("Invalid handler.");
        }
    }

    public async Task<IActionResult> OnPostSaveBudgetAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (SelectedBudget is not null)
        {
            var budgetId = HttpContext.Session.Get("SelectedBudgetId");
            if (budgetId != null && budgetId.Length == 16)
            {
                SelectedBudget.Id = new Guid(budgetId);
            }

            await _iBudgetManager.SaveBudgetAsync(SelectedBudget);
        }

        return RedirectToPage();
    }

    public async Task OnPostDeleteBudgetAsync()
    {
        if (Guid.TryParse(Request.Form["deleteBudgetId"], out Guid deleteBudgetId))
        {
            await _iBudgetManager.DeleteBudgetAsync(deleteBudgetId);
        }
    }

    public IActionResult OnPostAddCategoryAsync(string categoryType)
    {
        if (SelectedBudget == null)
        {
            return Page();
        }

        if (categoryType == "income")
        {
            if (SelectedBudget.Incomes == null || SelectedBudget.Incomes.Count < 1)
            {
                SelectedBudget.Incomes = new List<Category>();
            }

            SelectedBudget.Incomes.Add(new Category
            {
                Id = Guid.NewGuid(),
                Name = "New Income",
                Items = new List<Item>()
            });
        }
        else if (categoryType == "expense")
        {
            if (SelectedBudget.Expenses == null || SelectedBudget.Expenses.Count < 1)
            {
                SelectedBudget.Expenses = new List<Category>();
            }

            SelectedBudget.Expenses.Add(new Category
            {
                Id = Guid.NewGuid(),
                Name = "New Expense",
                Items = new List<Item>()
            });
        }
        else
        {
            throw new ArgumentException("Invalid category type.");
        }

        return Page();
    }


    public async Task<IActionResult> OnPostRemoveCategoryAsync(Guid categoryId)
    {
        if (SelectedBudget == null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId, item: null);
        SelectedBudget = _iBudgetManager.ReloadBudget(SelectedBudget);
        IsWorkingOnBudget = true;
        return Page();
    }

    public IActionResult OnPostAddItem(Guid categoryId)
    {
        var category = SelectedBudget!.Incomes!.Concat(SelectedBudget.Expenses!)
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(Guid categoryId, int itemIndex)
    {
        if (SelectedBudget == null)
        {
            return Page();
        }

        var removeItem = SelectedBudget?.Incomes?
            .Concat(SelectedBudget.Expenses!)?
            .FirstOrDefault(x => x.Id == categoryId)?
            .Items[itemIndex];
        
        if (removeItem is null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId: null, removeItem);
        SelectedBudget = _iBudgetManager.ReloadBudget(SelectedBudget);
        IsWorkingOnBudget = true;
        return Page();
    }

    private async Task<Budget> FillUpSelectedBudgetAsync()
    {
        var budgetId = RetrieveGuidIdFromSession();
        Budget budget = new Budget();
        if (budgetId != new Guid())
            budget = await _iBudgetManager.FetchBudgetAsync(budgetId);
        return budget;
    }

    private Guid RetrieveGuidIdFromSession()
    {
        var result = HttpContext.Session.Get("SelectedBudgetId");
        if (result != null && result.Length == 16)
            return new Guid(result);
        return Guid.Empty;
    }
}
