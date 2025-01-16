﻿using Microsoft.AspNetCore.Http;

namespace ThePersonalBudgetApp.DAL.Helpers;

public class GlobalMethods(IBudgetManager iBudgetManager)
{

    private IBudgetManager _iBudgetManager = iBudgetManager;

    public async Task<Budget?> FillUpSelectedBudgetAsync(HttpContext? httpContext = null, string? key = null)
    {
        if (key == null || httpContext == null)
            return null;

        var budgetId = RetrieveGuidIdFromSession(httpContext, key);
        Budget? budget = null;
        if (budgetId != Guid.Empty)
            budget = await _iBudgetManager.FetchBudgetAsync(budgetId);

        if (budget == null)
            return new Budget();

        return budget;
    }

    public Guid RetrieveGuidIdFromSession(HttpContext? httpContext, string? key = null)
    {
        if (httpContext == null || httpContext.Session == null)
            throw new InvalidOperationException("HttpContext or session is not available.");

        //var result = httpContext.Session.Get("SelectedBudgetId");
        if (key is null)
            key = "SelectedBudgetId";

        var result = httpContext!.Session.Get(key);

        if (result != null && result.Length == 16)
            return new Guid(result);

        return Guid.Empty;
    }
}
