using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThePersonalBudgetApp.Pages
{
    public class CreateBudgetModel : PageModel
    {
        [BindProperty]
        public Budget CreatedBudget { get; set; } = new Budget();

        private IBudgetManager _iBudgetManager;

        public CreateBudgetModel(IBudgetManager iBudgetManager)
        {
            _iBudgetManager = iBudgetManager;
        }

        public void OnGet()
        {
            if (CreatedBudget.Title == null)
            {
                CreatedBudget.Title = "Min Budget";
                CreatedBudget.Description = "Beskrivning";
            }
        }

        public async Task<IActionResult> OnPostSaveBudgetAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (CreatedBudget is not null)
            {
                var budgetId = HttpContext.Session.Get("SelectedBudgetId");
                if (budgetId != null && budgetId.Length == 16)
                {
                    CreatedBudget.Id = new Guid(budgetId);
                }

                await _iBudgetManager.SaveBudgetAsync(CreatedBudget);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostAddCategoryAsync(string categoryType)
        {
            if (CreatedBudget == null)
            {
                return Page();
            }

            if (categoryType == "income")
            {
                if (CreatedBudget.Incomes == null || CreatedBudget.Incomes.Count < 1)
                {
                    CreatedBudget.Incomes = new List<Category>();
                }

                CreatedBudget.Incomes.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "New Income",
                    Items = new List<Item>()
                });
            }
            else if (categoryType == "expense")
            {
                if (CreatedBudget.Expenses == null || CreatedBudget.Expenses.Count < 1)
                {
                    CreatedBudget.Expenses = new List<Category>();
                }

                CreatedBudget.Expenses.Add(new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "New Expense",
                    Items = new List<Item>()
                });
            }
            else
            {
                throw new ArgumentException("Invalid category type.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveCategoryAsync(Guid categoryId)
        {
            if (CreatedBudget == null)
            {
                return Page();
            }

            await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId, item: null);
            CreatedBudget = _iBudgetManager.ReloadBudget(CreatedBudget);
            return Page();
        }

        public IActionResult AddItem(Guid categoryId)
        {
            var category = CreatedBudget.Incomes!.Concat(CreatedBudget.Expenses!)
                .FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
            }

            return Page();
        }
    }
}
