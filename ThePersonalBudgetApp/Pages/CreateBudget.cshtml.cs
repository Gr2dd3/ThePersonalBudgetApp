namespace ThePersonalBudgetApp.Pages;

public class CreateBudgetModel : PageModel
{
    [BindProperty]
    public Budget CreatedBudget { get; set; }

    private IBudgetManager _iBudgetManager;
    private IHttpContextAccessor _httpContextAccessor;
    private string _sessionKey = "CreatedBudgetId";
    public CreateBudgetModel(IBudgetManager iBudgetManager, IHttpContextAccessor httpContextAccessor)
    {
        _iBudgetManager = iBudgetManager;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task OnGet()
    {
        var budgetId = GetId();
        if (budgetId != Guid.Empty)
        {
            CreatedBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
        }
        else
        {
            CreatedBudget = new Budget()
            {
                Id = Guid.NewGuid(),
                Title = "Min Budget",
                Description = "Beskrivning",
                Categories = new List<Category>(),

            };
            await OnPostSaveBudgetAsync(CreatedBudget.Id);
        }
    }

    public async Task<IActionResult> OnPostSaveFieldAsync([FromBody] FieldUpdateModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.FieldName))
        {
            return BadRequest("Invalid data.");
        }

        var category = CreatedBudget.Categories!.FirstOrDefault(c => c.Id == model.CategoryId);
        if (category == null)
        {
            return NotFound("Category not found.");
        }

        if (model.ItemId != null)
        {
            var item = category.Items!.FirstOrDefault(i => i.Id == model.ItemId);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            if (model.FieldName == "Amount")
            {
                item.Amount = float.TryParse(model.Value, out var parsedAmount) ? parsedAmount : 0;
            }
            else if (model.FieldName == "Name")
            {
                item.Name = model.Value;
            }
        }
        else if (model.FieldName == "Name")
        {
            category.Name = model.Value;
        }
        CreatedBudget.Id = GetId();
        await _iBudgetManager.SaveBudgetAsync(CreatedBudget);
        return new JsonResult(new { success = true });
    }


    public async Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        if (CreatedBudget is not null)
        {
            CreatedBudget.Id = GetId();
            if (CreatedBudget.Id == Guid.Empty)
            {
                CreatedBudget.Id = Guid.NewGuid();
                SetId(CreatedBudget.Id);
            }
            else if (budgetId is not null)
            {
                Guid confirmedId = (Guid)budgetId;
                SetId(confirmedId);
            }
            else
            {
                throw new Exception($"Unable to save budget with {CreatedBudget.Id}");

            }
            await _iBudgetManager.SaveBudgetAsync(CreatedBudget);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddCategoryNameAsync(Guid categoryId, string categoryName)
    {
        if (!ModelState.IsValid)
            return Page();

        await _iBudgetManager.SaveCategoryAsync(categoryId, categoryName);
        CreatedBudget = _iBudgetManager.ReloadBudget(CreatedBudget);

        return RedirectToPage();
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

        await Save();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveCategoryAsync(Guid categoryId, string? categoryName = null)
    {
        if (CreatedBudget == null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId, item: null);
        CreatedBudget = _iBudgetManager.ReloadBudget(CreatedBudget);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddItem(Guid categoryId)
    {
        var category = CreatedBudget.Categories!
            .FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            category.Items!.Add(new Item { Name = "New Item", Amount = 0 });
        }

        await Save();

        return RedirectToPage();
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
        CreatedBudget!.Id = GetId();
        CreatedBudget = _iBudgetManager.ReloadBudget(CreatedBudget!);
        return RedirectToPage();
    }

    #region Private Methods

    private async Task Save()
    {
        CreatedBudget.Id = GetId();
        await OnPostSaveBudgetAsync(CreatedBudget.Id);
    }

    private void SetId(Guid id) => _httpContextAccessor?.HttpContext?.Session.Set(_sessionKey, id.ToByteArray());
    private Guid GetId() => GlobalMethods.GetBudgetIdFromSessionAsync(_httpContextAccessor.HttpContext, _sessionKey);

    #endregion
}
