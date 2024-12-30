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
        //Budgets = HttpContext.Session.Get<List<Budget>>("Budgets");

        Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
        //if (Budgets is null || !Budgets.Any())
        //{
        //    Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
        //    HttpContext.Session.Set("Budgets", Budgets);
        //}
    }

    public async Task OnPostAsync()
    {
        if (Request.Form["action"] == "back")
        {
            IsWorkingOnBudget = false;
            SelectedBudget = null;
        }
        else if (Request.Form["handler"] == "SelectedBudget")
        {
            await OnPostSelectedBudgetAsync();
        }
        else
        {
            if (Guid.TryParse(Request.Form["selectedBudgetId"], out Guid budgetId))
            {
                SelectedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
                HttpContext.Session.Set("SelectedBudgetId", SelectedBudget.Id.ToByteArray());
                IsWorkingOnBudget = true;
            }
        }
    }

    public async Task<IActionResult> OnPostSelectedBudgetAsync()
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
}
