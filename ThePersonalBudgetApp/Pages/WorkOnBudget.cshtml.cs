using Microsoft.EntityFrameworkCore;
using ThePersonalBudgetApp.DAL.Models;

namespace ThePersonalBudgetApp.Pages;

public class WorkOnBudgetModel : PageModel, IBudgetHandler
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
    public Budget? CurrentBudget { get; set; }

    public async Task OnGetAsync()
    {
        Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Request.Form["action"] == "back")
        {
            IsWorkingOnBudget = false;
            CurrentBudget = null;
        }
        else if (Guid.TryParse(Request.Form[_sessionKey], out Guid budgetId))
        {
            CurrentBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
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

        if (CurrentBudget is not null)
        {
            if (budgetId == null)
                budgetId = GetId();

            Guid confirmedId = (Guid)budgetId;
            CurrentBudget.Id = budgetId == Guid.Empty ? Guid.NewGuid() : confirmedId;


            await _iBudgetManager.SaveBudgetAsync(CurrentBudget);
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
        if (CurrentBudget == null)
        {
            return Page();
        }

        CurrentBudget.Categories!.Add(new Category
        {
            Id = Guid.NewGuid(),
            Name = "New Category",
            IsIncome = categoryType == "income" ? true : false,
            Items = new List<Item>()
        });

        await Save();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveCategoryAsync(Guid categoryId, string? categoryName = null)
    {
        if (CurrentBudget == null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId, item: null);
        CurrentBudget = _iBudgetManager.ReloadBudget(CurrentBudget);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddItem(Guid categoryId)
    {
        if (categoryId != null)
        {
            var category = CurrentBudget!.Categories!
                .FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
                category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
        }
        await Save();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(Guid categoryId, int itemIndex)
    {
        if (CurrentBudget == null)
        {
            return Page();
        }

        var removeFromCategory = CurrentBudget?.Categories!
            .FirstOrDefault(x => x.Id == categoryId);
        var removeItem = removeFromCategory?
            .Items![itemIndex];

        if (removeItem is null)
        {
            return Page();
        }
        if (itemIndex < 0 || itemIndex >= removeFromCategory?.Items!.Count)
        {
            return BadRequest("Invalid item index.");
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId: null, removeItem);
        CurrentBudget = _iBudgetManager.ReloadBudget(CurrentBudget!);
        IsWorkingOnBudget = true;
        return RedirectToPage();
    }

    #region Private Methods
    private bool IsBudgetValid()
    {
        return CurrentBudget != null && CurrentBudget.Categories != null;
    }

    private async Task Save()
    {
        CurrentBudget.Id = GetId();
        await OnPostSaveBudgetAsync(CurrentBudget.Id);
    }
    private void SetId(Guid id) => _httpContextAccessor?.HttpContext?.Session.Set(_sessionKey, id.ToByteArray());
    private Guid GetId()
    {
        var id = GlobalMethods.GetBudgetIdFromSessionAsync(_httpContextAccessor.HttpContext, _sessionKey);
        if (id == Guid.Empty)
        {
            id = Guid.NewGuid();
            SetId(id);
        }
        return id;
    }

    #endregion

}
