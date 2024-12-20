
namespace ThePersonalBudgetApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Budget Budget { get; set; } = new Budget();

    private IBudgetManager _budgetManager;

    public IndexModel(IBudgetManager budgetManager)
    {
        _budgetManager = budgetManager;
    }

    public void OnGet()
    {
        //TODO: Create menu that lets you choose among budgets to fetch or delete or new
        if (Budget.Title == null)
        {
            Budget.Title = "Min Budget";
            Budget.Description = "Beskrivning";
        }

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Handle the budget data here
        await _budgetManager.SaveBudgetAsync(Budget);
        // Example: Send Budget to BudgetManager
        TempData["Message"] = "Budget saved successfully!";
        return RedirectToPage();
    }
}

