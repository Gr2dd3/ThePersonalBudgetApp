using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ThePersonalBudgetApp.DAL.Helpers;

namespace ThePersonalBudgetApp.Pages
{
    public class CreateBudgetModel : PageModel
    {
        [BindProperty]
        public Budget CreatedBudget { get; set; } = new Budget();

        private IBudgetManager _iBudgetManager;
        private GlobalMethods _globalMethods;
        private IHttpContextAccessor _httpContextAccessor;
        private string _sessionKey = "CreatedBudgetId";
        public CreateBudgetModel(IBudgetManager iBudgetManager, GlobalMethods globalMethods, IHttpContextAccessor httpContextAccessor)
        {
            _iBudgetManager = iBudgetManager;
            _globalMethods = globalMethods;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnGet()
        {
            var budget = await _globalMethods.FillUpSelectedBudgetAsync(_httpContextAccessor.HttpContext, _sessionKey);
            if (budget!.Id != Guid.Empty)
            {
                CreatedBudget = budget;
            }
            else
            {
                CreatedBudget!.Title = "Min Budget";
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
                //TODO 13/1 Sparar budget n�r man skapar n�got. Vart b�r vi h�mta tillbaka den n�r sidan h�mtas igen?
                if (CreatedBudget.Id == Guid.Empty)
                {
                    CreatedBudget.Id = Guid.NewGuid();
                    _httpContextAccessor.HttpContext.Session.Set(_sessionKey, CreatedBudget.Id.ToByteArray());
                    // To fetch budget from session see DAL.Helpers.GlobalMethods
                }
                else
                {
                    CreatedBudget = await _globalMethods.FillUpSelectedBudgetAsync(_httpContextAccessor.HttpContext, _sessionKey);
                }

                await _iBudgetManager.SaveBudgetAsync(CreatedBudget);
            }

            return Page();
        }

        public IActionResult OnPostAddCategoryAsync(string categoryType)
        {
            if (CreatedBudget == null)
            {
                return Page();
            }

            if (categoryType == "income")
            {
                if (CreatedBudget.Incomes == null)
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
                if (CreatedBudget.Expenses == null)
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
            // Save CreatedBudget
            _ = OnPostSaveBudgetAsync();
            return RedirectToPage();
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

        public IActionResult OnPostAddItem(Guid categoryId)
        {
            var category = CreatedBudget.Incomes!.Concat(CreatedBudget.Expenses!)
                .FirstOrDefault(c => c.Id == categoryId);
            if (category != null)
            {
                category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(Guid categoryId, int itemIndex)
        {
            if (CreatedBudget == null)
            {
                return Page();
            }

            var removeItem = CreatedBudget?.Incomes?
                .Concat(CreatedBudget.Expenses!)?
                .FirstOrDefault(x => x.Id == categoryId)?
                .Items![itemIndex];

            if (removeItem is null)
            {
                return Page();
            }

            await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId: null, removeItem);
            CreatedBudget = _iBudgetManager.ReloadBudget(CreatedBudget!);
            return Page();
        }
    }
}
