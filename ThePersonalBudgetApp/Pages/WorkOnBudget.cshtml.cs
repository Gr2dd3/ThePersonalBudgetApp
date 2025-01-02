using Microsoft.EntityFrameworkCore;

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
        else if (Request.Form["handler"] == "AddCategory")
        {
            var categoryType = Request.Form["categoryType"].ToString();
            OnPostAddCategory(categoryType);
        }
        else if (Request.Form["handler"] == "RemoveCategory")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId))
            {
                OnPostRemoveCategory(categoryId);
            }
        }
        else if (Request.Form["handler"] == "AddItem")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId))
            {
                OnPostAddItem(categoryId);
            }
        }
        else if (Request.Form["handler"] == "RemoveItem")
        {
            if (Guid.TryParse(Request.Form["categoryId"], out Guid categoryId) &&
                int.TryParse(Request.Form["itemIndex"], out int itemIndex))
            {
                OnPostRemoveItem(categoryId, itemIndex);
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

    public IActionResult OnPostAddCategory(string categoryType)
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


    public IActionResult OnPostRemoveCategory(Guid categoryId)
    {
        if (SelectedBudget == null)
        {
            return Page(); 
        }

        var incomeCategory = SelectedBudget.Incomes.FirstOrDefault(c => c.Id == categoryId);
        if (incomeCategory != null)
        {
            SelectedBudget.Incomes.Remove(incomeCategory);
        }
        else
        {
            var expenseCategory = SelectedBudget.Expenses.FirstOrDefault(c => c.Id == categoryId);
            if (expenseCategory != null)
            {
                SelectedBudget.Expenses.Remove(expenseCategory);
            }
            else
            {
                throw new Exception("Category not found.");
            }
        }

        return Page();
    }


    public IActionResult OnPostAddItem(Guid categoryId)
    {
        var category = SelectedBudget.Incomes.Concat(SelectedBudget.Expenses)
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            category.Items.Add(new Item { Name = "New Item", Amount = 0 });
        }

        return Page();
    }

    public IActionResult OnPostRemoveItem(Guid categoryId, int itemIndex)
    {
        var category = SelectedBudget.Incomes.Concat(SelectedBudget.Expenses)
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null && itemIndex >= 0 && itemIndex < category.Items.Count)
        {
            category.Items.RemoveAt(itemIndex);
        }

        return Page();
    }
}
