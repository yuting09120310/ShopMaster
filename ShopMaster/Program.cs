using Microsoft.EntityFrameworkCore;
using ShopMaster.Areas.BackEnd.Models;

var builder = WebApplication.CreateBuilder(args);

// 設定 MySQL 連線
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//builder.Services.AddDbContext<ShopMaster.Areas.BackEnd.Models.shopmasterdbContext>(options =>
//    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));

builder.Services.AddDbContext<shopmasterdbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));

builder.Services.AddSession(); // 啟用 Session
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=HomeFront}/{action=Index}/{id?}",
    defaults: new { area = "FrontEnd" });

app.Run();
