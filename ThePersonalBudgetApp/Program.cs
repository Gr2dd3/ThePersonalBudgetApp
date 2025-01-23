namespace ThePersonalBudgetApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<IBudgetManager, BudgetManager>();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddRazorPages();

        ////Currently not using DI, to see whats wrong with db connection
        //builder.Services.AddDbContext<BudgetDbContext>(options =>
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
        //    {
        //        sqlOptions.CommandTimeout(60);
        //    })
        //    // Remove these in production:
        //    .EnableSensitiveDataLogging()
        //    .EnableDetailedErrors());


        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });


        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    if (exception != null)
                    {
                        Console.WriteLine($"Unhandled exception: {exception.Message}");
                        Console.WriteLine($"Stack trace: {exception.StackTrace}");
                    }

                    context.Response.Redirect("/Error");
                });
            });
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();
        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapDefaultControllerRoute();

        app.Run();
    }
}
