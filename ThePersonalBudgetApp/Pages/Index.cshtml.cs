namespace ThePersonalBudgetApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Budget Budget { get; set; } = new Budget();

    public void OnGet()
    {
        if (Budget.Title == null)
        {
            Budget.Title = "Min Budget";
            Budget.Description = "Beskrivning";
        }

    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Handle the budget data here
        // Example: Send Budget to BudgetManager
        TempData["Message"] = "Budget saved successfully!";
        return RedirectToPage();
    }
}

