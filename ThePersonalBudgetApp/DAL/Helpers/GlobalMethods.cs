using Microsoft.AspNetCore.Http;

namespace ThePersonalBudgetApp.DAL.Helpers;

public static class GlobalMethods
{
    // Change name to GetBudgetIdFromSessionAsync. Make it to one method
    public static Guid GetBudgetIdFromSessionAsync(HttpContext? httpContext = null, string? key = null)
    {
        if (httpContext == null || httpContext.Session == null)
            return Guid.Empty;

        if (key is null)
            key = "SelectedBudgetId";

        var result = httpContext!.Session.Get(key);

        if (result != null && result!.Length == 16)
            return new Guid(result);

        return Guid.Empty;
    }

    //public static async Task<Budget?> FillUpSelectedBudgetAsync(HttpContext? httpContext = null, string? key = null, IBudgetManager? iBudgetManager = null)
    //{
    //    if (key == null || httpContext == null)
    //        return null;

    //    var budgetId = RetrieveGuidIdFromSession(httpContext, key);
    //    Budget? budget = null;
    //    if (budgetId != Guid.Empty)
    //        budget = await iBudgetManager.FetchBudgetAsync(budgetId);

    //    if (budget == null)
    //        return new Budget();

    //    return budget;
    //}
}
