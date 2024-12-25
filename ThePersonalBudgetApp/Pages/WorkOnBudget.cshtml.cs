namespace ThePersonalBudgetApp.Pages;

public class WorkOnBudgetModel : PageModel
{
    [BindProperty]
    public bool IsWorkingOnBudget { get; set; } = false;
    public void OnGet()
    {
    }
}
