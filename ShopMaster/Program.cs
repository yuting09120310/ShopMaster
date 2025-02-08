var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSession(); 

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "backend",
    pattern: "BackEnd/{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "BackEnd" });

app.MapControllerRoute(
    name: "frontend",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "FrontEnd" });

app.Run();
