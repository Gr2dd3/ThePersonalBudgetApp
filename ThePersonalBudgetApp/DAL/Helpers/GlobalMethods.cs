using Microsoft.AspNetCore.Http;

namespace ThePersonalBudgetApp.DAL.Helpers;

public class GlobalMethods
{
    private IBudgetManager _iBudgetManager;
    public GlobalMethods(IBudgetManager iBudgetManager)
    {
        _iBudgetManager = iBudgetManager;
    }

    public async Task<Budget> FillUpSelectedBudgetAsync(HttpContext httpContext)
    {
        var budgetId = RetrieveGuidIdFromSession(httpContext);
        Budget budget = new Budget();
        if (budgetId != new Guid())
            budget = await _iBudgetManager.FetchBudgetAsync(budgetId);
        return budget;
    }

    public Guid RetrieveGuidIdFromSession(HttpContext httpContext)
    {
        var result = httpContext.Session.Get("SelectedBudgetId");
        if (result != null && result.Length == 16)
            return new Guid(result);
        return Guid.Empty;
    }
}
