namespace ThePersonalBudgetApp.Pages;

[IgnoreAntiforgeryToken]
public class CreateBudgetModel : PageModel, IBudgetHandler
{
    [BindProperty]
    public Budget CurrentBudget { get; set; }

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
            CurrentBudget = await _iBudgetManager.FetchBudgetAsync(budgetId);
        }
        else
        {
            CurrentBudget = new Budget()
            {
                Id = Guid.NewGuid(),
                Title = "Min Budget",
                Description = "Beskrivning",
                Categories = new List<Category>(),

            };
            await OnPostSaveBudgetAsync(CurrentBudget.Id);
        }
    }

    //TODO: Test to see if we can reach this method. site.js seems fine now. response = 200 OK
    
    public async Task<IActionResult> OnPostSaveFieldAsync([FromBody] FieldUpdateModel model)
    {
        var iId = Guid.TryParse(model.ItemId, out var itemId);
        var cId = Guid.TryParse(model.CategoryId, out var categoryId);
        #region SafetyChecks

        var options = new JsonSerializerOptions { WriteIndented = true };
        var requestBody = System.Text.Json.JsonSerializer.Serialize(model, options);
        Console.WriteLine($"Received request: {requestBody}");

        if (model == null || string.IsNullOrEmpty(model.FieldName))
        {
            return BadRequest("Invalid data.");
        }
        if (!IsBudgetValid())
        {
            return BadRequest("Budget or categories not loaded properly.");
        }
        if (!Guid.TryParse(categoryId.ToString(), out _))
        {
            return BadRequest("Invalid CategoryId GUID.");
        }

        if (itemId != Guid.Empty && !Guid.TryParse(itemId.ToString(), out _))
        {
            return BadRequest("Invalid ItemId GUID.");
        }
        #endregion

        

        //New Item?
        if (itemId != Guid.Empty)
        {
            float itemAmount = 0;
            var itemName = string.Empty;
            if (model.FieldName == "Amount")
            {
                itemAmount = float.TryParse(model.Value, out var parsedAmount) ? parsedAmount : 0;
            }
            else if (model.FieldName == "Name")
            {
                itemName = model.Value;
            }
            await _iBudgetManager.SaveItemAsync(categoryId, itemId, itemName, itemAmount);
        }
        else if (model.FieldName == "Name")
        {
            string categoryName = string.Empty;
            if (model.Value is not null)
                categoryName = model.Value;
            await _iBudgetManager.SaveCategoryAsync(categoryId, categoryName);
        }
        CurrentBudget.Id = GetId();
        await _iBudgetManager.SaveBudgetAsync(CurrentBudget);
        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostSaveBudgetAsync(Guid? budgetId = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        if (CurrentBudget is not null)
        {
            CurrentBudget.Id = GetId();
            if (CurrentBudget.Id == Guid.Empty)
            {
                CurrentBudget.Id = Guid.NewGuid();
                SetId(CurrentBudget.Id);
            }
            else if (budgetId is not null)
            {
                Guid confirmedId = (Guid)budgetId;
                SetId(confirmedId);
            }
            else
            {
                throw new Exception($"Unable to save budget with {CurrentBudget.Id}");

            }
            await _iBudgetManager.SaveBudgetAsync(CurrentBudget);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddCategoryNameAsync(Guid categoryId, string categoryName)
    {
        if (!ModelState.IsValid)
            return Page();

        await _iBudgetManager.SaveCategoryAsync(categoryId, categoryName);
        CurrentBudget = _iBudgetManager.ReloadBudget(CurrentBudget);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddCategoryAsync(string categoryType)
    {
        if (CurrentBudget == null)
        {
            return Page();
        }

        CurrentBudget.Categories!.Add(new Category()
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
        if (CurrentBudget == null)
        {
            return Page();
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId, item: null);
        CurrentBudget = _iBudgetManager.ReloadBudget(CurrentBudget);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddItem(Guid categoryId)
    {
        var category = CurrentBudget.Categories!
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
        if (CurrentBudget == null)
        {
            return Page();
        }
        var removeFromCategory = CurrentBudget?.Categories!
            .FirstOrDefault(x => x.Id == categoryId);
        var removeItem = removeFromCategory?
            .Items![itemIndex];

        if (removeItem is null)
        {
            return Page();
        }
        if (itemIndex < 0 || itemIndex >= removeFromCategory?.Items!.Count)
        {
            return BadRequest("Invalid item index.");
        }

        await _iBudgetManager.DeleteBudgetCategoryOrItemAsync(categoryId: null, removeItem);
        CurrentBudget!.Id = GetId();
        CurrentBudget = _iBudgetManager.ReloadBudget(CurrentBudget!);
        return RedirectToPage();
    }

    #region Private Methods
    private bool IsBudgetValid()
    {
        return CurrentBudget != null && CurrentBudget.Categories != null;
    }
    private async Task Save()
    {
        CurrentBudget.Id = GetId();
        await OnPostSaveBudgetAsync(CurrentBudget.Id);
    }

    private void SetId(Guid id) => _httpContextAccessor?.HttpContext?.Session.Set(_sessionKey, id.ToByteArray());
    private Guid GetId() => GlobalMethods.GetBudgetIdFromSessionAsync(_httpContextAccessor.HttpContext, _sessionKey);


    #endregion
}
