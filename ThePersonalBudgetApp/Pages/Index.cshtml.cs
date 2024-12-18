

//namespace ThePersonalBudgetApp.Pages;

//public class IndexModel : PageModel
//{
//    private readonly ILogger<IndexModel> _logger;

//    [BindProperty]
//    Budget Budget { get; set; }

//    BudgetManager _budgetManager;

//    public IndexModel(ILogger<IndexModel> logger, BudgetManager budgetManager)
//    {
//        _logger = logger;
//        _budgetManager = budgetManager;
//    }

//    public void OnGet()
//    {

//    }
//}

namespace ThePersonalBudgetApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public Budget Budget { get; set; } = new Budget();

    public void OnGet()
    {
        if (Budget.Title == null)
        {
            Budget.Title = "My Budget";
            Budget.Description = "Example description";
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

