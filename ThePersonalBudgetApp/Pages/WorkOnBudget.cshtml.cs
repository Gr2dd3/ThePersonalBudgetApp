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

    public async Task OnGetAsync()
    {
        Budgets = HttpContext.Session.Get<List<Budget>>("Budgets");

        if (Budgets is null || !Budgets.Any())
        { 
            Budgets = await _iBudgetManager.FetchAllBudgetsAsync();
            HttpContext.Session.Set("Budgets", Budgets);
        }
    }

    public async Task OnPostAsync()
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
