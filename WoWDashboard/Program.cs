using WoWDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BlizzardService>();
builder.Services.AddSingleton<RaiderIOService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Character}/{action=Index}/{id?}");

app.Run();
