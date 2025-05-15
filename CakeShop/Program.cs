using CakeShop.Data;
using CakeShop.Models; // 確保 using 了你的模型命名空間
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); // 或者 UseSqlite, UseNpgsql 等
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ====================> 檢查並修改這一區塊 <====================
// **重點：** 確保 AddDefaultIdentity 或 AddIdentity 的泛型參數是 ApplicationUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options => { // <-- 指定 ApplicationUser
                                                                  // 在這裡設定 Identity 選項，例如是否需要確認 Email，密碼規則等
    options.SignIn.RequireConfirmedAccount = false; // 開發時設為 false 較方便
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    // **重點：** 確保連結到你的 ApplicationDbContext
    .AddEntityFrameworkStores<ApplicationDbContext>();
// ==============================================================

// 如果你需要使用角色管理 (Roles)，則改用 AddIdentity
// builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => { // <-- 指定 ApplicationUser 和 IdentityRole
//         options.SignIn.RequireConfirmedAccount = false;
//         // ... 其他選項 ...
//     })
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders(); // AddIdentity 通常需要這個

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // 如果你有使用 Razor Pages (像 Identity UI 就是)，需要這行

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ====================> 檢查 Middleware 順序 <====================
// **重點：** UseAuthentication 必須在 UseAuthorization 之前
app.UseAuthentication(); // 啟用驗證功能
app.UseAuthorization(); // 啟用授權功能
// ==============================================================


// **重點：** 確保 MapRazorPages() 被呼叫，這樣 Identity 的頁面才能運作
app.MapRazorPages(); // 映射 Identity UI (Razor Pages) 的路由

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 資料庫初始化 (如果有的話)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // context.Database.Migrate(); // 可選
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


app.Run();