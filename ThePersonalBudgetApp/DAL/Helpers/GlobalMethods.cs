using Microsoft.AspNetCore.Http;

namespace ThePersonalBudgetApp.DAL.Helpers;

public static class GlobalMethods
{
    public static async Task<Budget?> FillUpSelectedBudgetAsync(HttpContext? httpContext = null, string? key = null, IBudgetManager? iBudgetManager = null)
    {
        if (key == null || httpContext == null)
            return null;

        var budgetId = RetrieveGuidIdFromSession(httpContext, key);
        Budget? budget = null;
        if (budgetId != Guid.Empty)
            budget = await iBudgetManager.FetchBudgetAsync(budgetId);

        if (budget == null)
            return new Budget();

        return budget;
    }

    public static Guid RetrieveGuidIdFromSession(HttpContext? httpContext, string? key = null)
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



    //public static async Task TestingDbConnection(Guid id)
    //{
    //    var connectionString = "Server=LAPTOP-HE47776N\\SQLEXPRESS;Database=MyBudget;Trusted_Connection=True;TrustServerCertificate=True;";

    //    var options = new DbContextOptionsBuilder<BudgetDbContext>()
    //    .UseSqlServer(connectionString)
    //    .Options;

    //    using (var context = new BudgetDbContext(options))
    //    {
    //        try
    //        {
    //            var budget = await context.Budgets.FindAsync(id);
    //            Console.WriteLine($"Found Budget: {budget?.Title}");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Error: {ex.Message}");
    //        }
    //    }
    //}
    public static async Task TestingDbConnection(DbContext context)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                Console.WriteLine("Connection failed.");
            }
            else
            {
                Console.WriteLine("Connection is open.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while checking database connection: {ex.Message}");
        }
    }

}
