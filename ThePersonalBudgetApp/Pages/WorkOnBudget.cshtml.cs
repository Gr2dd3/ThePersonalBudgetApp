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
        else if (Request.Form["handler"] == "SaveBudget")
        {
            await SaveBudgetAsync();
        }
        else if (Request.Query["handler"] == "AddCategory")
        {
            var categoryType = Request.Form["categoryType"].ToString();
            AddCategory(categoryType);
        }
        else if (Request.Query["handler"] == "RemoveCategory")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId))
            {
                OnPostRemoveCategoryAsync(categoryId);
            }
        }
        else if (Request.Query["handler"] == "AddItem")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId))
            {
                AddItem(categoryId);
            }
        }
        else if (Request.Query["handler"] == "RemoveItem")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId) &&
                int.TryParse(Request.Form["itemIndex"], out int itemIndex))
            {
                RemoveItem(categoryId, itemIndex);
            }
        }
        else
        {
            throw new Exception("Invalid handler.");
        }
    }

    public async Task<IActionResult> SaveBudgetAsync()
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

    public async Task OnPostDeleteAsync()
    {
        if (Guid.TryParse(Request.Form["deleteBudgetId"], out Guid deleteBudgetId))
        {
            await _iBudgetManager.DeleteBudgetAsync(deleteBudgetId);
        }
    }

    public IActionResult AddCategory(string categoryType)
    {
        if (SelectedBudget == null)
        {
            return Page();
        }

        if (categoryType == "Income")
        {
            SelectedBudget.Incomes.Add(new Category
            {
                Id = Guid.NewGuid(),
                Name = "New Income",
                Items = new List<Item>()
            });
        }
        else if (categoryType == "Expense")
        {
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
        SelectedBudget = await FillUpSelectedBudgetAsync();
        var incomeCategory = SelectedBudget.Incomes?.FirstOrDefault(c => c.Id == categoryId);
        if (incomeCategory != null)
        {
            SelectedBudget.Incomes?.Remove(incomeCategory);
        }
        else
        {
            var expenseCategory = SelectedBudget.Expenses?.FirstOrDefault(c => c.Id == categoryId);
            if (expenseCategory != null)
            {
                SelectedBudget.Expenses?.Remove(expenseCategory);
            }
            else
            {
                throw new Exception("Category not found.");
            }
        }
        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId);
        SelectedBudget = await _iBudgetManager.ReloadBudget(SelectedBudget);
;
        IsWorkingOnBudget = true;
        return Page();
    }

    public IActionResult AddItem(Guid categoryId)
    {
        var category = SelectedBudget.Incomes.Concat(SelectedBudget.Expenses)
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            category.Items.Add(new Item { Name = "New Item", Amount = 0 });
        }

        return Page();
    }

    public IActionResult RemoveItem(Guid categoryId, int itemIndex)
    {
        var category = SelectedBudget.Incomes.Concat(SelectedBudget.Expenses)
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null && itemIndex >= 0 && itemIndex < category.Items.Count)
        {
            category.Items.RemoveAt(itemIndex);
        }

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
