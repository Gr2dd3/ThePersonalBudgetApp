using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ThePersonalBudgetApp.DAL.Helpers;

namespace ThePersonalBudgetApp.Pages
{
    public class CreateBudgetModel : PageModel
    {
        [BindProperty]
        public Budget CreatedBudget { get; set; } = new Budget();

        // TODO: Include Incomes and Expenses list to pageview 
        public List<Category> Incomes { get; set; } = new List<Category>();
        public List<Category> Expenses { get; set; } = new List<Category>();

        private IBudgetManager _iBudgetManager;
        private IHttpContextAccessor _httpContextAccessor;
        private string _sessionKey = "CreatedBudgetId";
        public CreateBudgetModel(IBudgetManager iBudgetManager, IHttpContextAccessor httpContextAccessor)
        {
            _iBudgetManager = iBudgetManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnGet()
        {
            var budget = await GlobalMethods.FillUpSelectedBudgetAsync(_httpContextAccessor.HttpContext, _sessionKey, _iBudgetManager);
            if (budget!.Id != Guid.Empty)
            {
                CreatedBudget = budget;
                if (CreatedBudget != null)
                {
                    Incomes = CreatedBudget.Categories!.Where(c => c.IsIncome).ToList();
                    Expenses = CreatedBudget.Categories!.Where(c => !c.IsIncome).ToList();
                }
            }
            else
            {
                //ID för CreatedBudget?
                CreatedBudget = new Budget()
                {
                    Id = Guid.NewGuid(),
                    Title = "Min Budget",
                    Description = "Beskrivning",
                    Categories = new List<Category>(),

                };
                _ = OnPostSaveBudgetAsync(CreatedBudget.Id);
                _httpContextAccessor.HttpContext.Session.Set(_sessionKey, CreatedBudget.Id.ToByteArray());
            }
        }

        public async Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (CreatedBudget is not null)
            {
                if (CreatedBudget.Id == Guid.Empty)
                {
                    CreatedBudget.Id = Guid.NewGuid();
                    _httpContextAccessor.HttpContext.Session.Set(_sessionKey, CreatedBudget.Id.ToByteArray());
                }
                else if (budgetId is not null)
                {
                    Guid confirmedId = (Guid)budgetId;
                    _httpContextAccessor.HttpContext.Session.Set(_sessionKey, confirmedId.ToByteArray());
                }
                else
                {
                    throw new Exception($"Unable to save budget with {CreatedBudget.Id}");

                }
                await _iBudgetManager.SaveBudgetAsync(CreatedBudget);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddCategoryAsync(string categoryType)
        {
            if (CreatedBudget == null)
            {
                return Page();
            }

            CreatedBudget.Categories!.Add(new Category()
            {
                Id = Guid.NewGuid(),
                Name = "New Category",
                IsIncome = categoryType == "income" ? true : false,
                Items = new List<Item>()
            });

            // Save CreatedBudget
            await OnPostSaveBudgetAsync();
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
            var category = CreatedBudget.Categories!
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

            var removeItem = CreatedBudget?.Categories!
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
