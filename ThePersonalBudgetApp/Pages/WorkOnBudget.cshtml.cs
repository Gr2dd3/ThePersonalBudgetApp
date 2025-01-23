using Microsoft.EntityFrameworkCore;
using ThePersonalBudgetApp.DAL.Models;

namespace ThePersonalBudgetApp.Pages;

public class WorkOnBudgetModel : PageModel
{
    private IBudgetManager _iBudgetManager;
    private IHttpContextAccessor _httpContextAccessor;
    private string _sessionKey = "selectedBudgetId";
    public WorkOnBudgetModel(IBudgetManager budgetManager, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
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
        else if (Guid.TryParse(Request.Form[_sessionKey], out Guid budgetId))
        {
            SelectedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
            SetId(budgetId);
            IsWorkingOnBudget = true;
        }
        else
        {
            throw new Exception("Invalid handler.");
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (SelectedBudget is not null)
        {
            if (budgetId == null)
                budgetId = GetId();

            Guid confirmedId = (Guid)budgetId;
            SelectedBudget.Id = budgetId == Guid.Empty ? Guid.NewGuid() : confirmedId;


            await _iBudgetManager.SaveBudgetAsync(SelectedBudget);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteBudgetAsync(Guid deleteBudgetId)
    {
        await _iBudgetManager.DeleteBudgetAsync(deleteBudgetId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddCategoryAsync(string categoryType)
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

        await Save();
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

    public async Task<IActionResult> OnPostAddItem(Guid? categoryId)
    {
        if (categoryId != null)
        {
            var category = SelectedBudget!.Categories!
                .FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
                category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
        }
        await Save();
        return RedirectToPage();
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
        return RedirectToPage();
    }

    #region Private Methods

    private async Task Save()
    {
        SelectedBudget.Id = GetId();
        await OnPostSaveBudgetAsync(SelectedBudget.Id);
    }
    private void SetId(Guid id) => _httpContextAccessor?.HttpContext?.Session.Set(_sessionKey, id.ToByteArray());
    private Guid GetId() => GlobalMethods.GetBudgetIdFromSessionAsync(_httpContextAccessor.HttpContext, _sessionKey);

    #endregion

}
