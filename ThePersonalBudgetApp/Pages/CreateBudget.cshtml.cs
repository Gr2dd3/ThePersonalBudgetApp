using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThePersonalBudgetApp.Pages
{
    public class CreateBudgetModel : PageModel
    {
        [BindProperty]
        public Budget Budget { get; set; } = new Budget();

        private IBudgetManager _iBudgetManager;

        public CreateBudgetModel(IBudgetManager iBudgetManager)
        {
            _iBudgetManager = iBudgetManager;
        }

        public void OnGet()
        {
            if (Budget.Title == null)
            {
                Budget.Title = "Min Budget";
                Budget.Description = "Beskrivning";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //TODO: Kolla datan som skickas tillbaka och sparas för ny budget. Den hämtas inte riktigt just nu till att arbeta med.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle the budget data here
            await _iBudgetManager.SaveBudgetAsync(Budget);

            return RedirectToPage();
        }
    }
}
