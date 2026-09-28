using HeimevernetInnlevering1.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=Data/heimevernet.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Create the SQLite directory/database automatically for local and Docker runs.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var databaseDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
    Directory.CreateDirectory(databaseDirectory);
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// New browser sessions must complete the form before using the rest of the site.
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var isFormPage = path.StartsWithSegments("/Home/Form");
    var isErrorPage = path.StartsWithSegments("/Home/Error");
    var isRegistered = context.Session.GetInt32("RegistrationId").HasValue;

    if (path.StartsWithSegments("/Home") && !isFormPage && !isErrorPage && !isRegistered)
    {
        context.Response.Redirect("/Home/Form");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
