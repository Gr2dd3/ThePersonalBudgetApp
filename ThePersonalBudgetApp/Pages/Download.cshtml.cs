using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThePersonalBudgetApp.Pages
{
    public class DownloadModel : PageModel
    {
        public void OnGet()
        {
            //Fetch list of budgets
            //_budgetManager.FetchBudgets();


            //Send chosen one to _budgetManager.PrintToPDFAsync(budgetId);
        }
    }
}
