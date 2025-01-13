using Microsoft.AspNetCore.Http;

namespace ThePersonalBudgetApp.DAL.Helpers;

public class GlobalMethods(IBudgetManager iBudgetManager)
{

    private IBudgetManager _iBudgetManager = iBudgetManager;

    public async Task<Budget> FillUpSelectedBudgetAsync(HttpContext httpContext, string key = null)
    {
        var budgetId = RetrieveGuidIdFromSession(httpContext, key);
        Budget budget = new Budget();
        if (budgetId != new Guid())
            budget = await _iBudgetManager.FetchBudgetAsync(budgetId);
        return budget;
    }

    public Guid RetrieveGuidIdFromSession(HttpContext httpContext, string key = null)
    {
        //var result = httpContext.Session.Get("SelectedBudgetId");
        if (key is null)
        {
            key = "SelectedBudgetId";
        }
        var result = httpContext.Session.Get(key);
        if (result != null && result.Length == 16)
            return new Guid(result);
        return Guid.Empty;
    }
}
