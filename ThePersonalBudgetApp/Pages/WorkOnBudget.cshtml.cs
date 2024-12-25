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

    public Budget? SelectedBudget { get; private set; }

    public async Task OnGet()
    {
        Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
    }

    public async Task OnPost()
    {
        if (Request.Form["action"] == "back")
        {
            IsWorkingOnBudget = false;
            SelectedBudget = null;
        }
        else
        {
            if (Guid.TryParse(Request.Form["selectedBudgetId"], out Guid budgetId))
            {
                SelectedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
                IsWorkingOnBudget = true;
            }
        }
    }
}
