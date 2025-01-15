using Microsoft.EntityFrameworkCore;
using ThePersonalBudgetApp.DAL.Models;

namespace ThePersonalBudgetApp.Pages;

public class WorkOnBudgetModel : PageModel
{
    private IBudgetManager _iBudgetManager;
    private string _key = "selectedBudgetId";
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (Request.Form["action"] == "back")
        {
            IsWorkingOnBudget = false;
            SelectedBudget = null;
        }
        else if (Guid.TryParse(Request.Form[_key], out Guid budgetId))
        {
            SelectedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
            HttpContext.Session.Set(_key, SelectedBudget.Id.ToByteArray());
            IsWorkingOnBudget = true;
        }
        else
        {
            throw new Exception("Invalid handler.");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostSaveBudgetAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (SelectedBudget is not null)
        {
            var budgetId = HttpContext.Session.Get(_key);
            if (budgetId != null && budgetId.Length == 16)
            {
                SelectedBudget.Id = new Guid(budgetId);
            }
            // Is every method persistant against Page Reload? Save in end of every method?
            //Is there a need to fill up budget in OnGet?
            // Is budget filled?
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

        SelectedBudget.Categories!.Add(new Category
        {
            Id = Guid.NewGuid(),
            Name = "New Category",
            IsIncome = categoryType == "income" ? true : false,
            Items = new List<Item>()
        });

        return RedirectToPage();
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
        return RedirectToPage();
    }

    public IActionResult OnPostAddItem(Guid categoryId)
    {
        var category = SelectedBudget!.Categories!
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

        var removeItem = SelectedBudget?.Categories!
            .FirstOrDefault(x => x.Id == categoryId)?
            .Items![itemIndex];

        if (removeItem is null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId: null, removeItem);
        SelectedBudget = _iBudgetManager.ReloadBudget(SelectedBudget!);
        IsWorkingOnBudget = true;
        return Page();
    }


}
