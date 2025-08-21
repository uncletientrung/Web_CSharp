using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Web_CSharp.Data;
using Web_CSharp.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Hshop2023Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("HShop"));
});

// Làm viec voi Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(2); // Thiet lap session trong bao lâu VD 2 là 2 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Dang Ky Map
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Dang Nhap Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/KhachHang/DangNhap";
    option.AccessDeniedPath = "/AccessDenied";
});

// Dang ky PaypalClient dạng Singleton() - chỉ có 1 istance duy nhất trong toàn ứng dụng
builder.Services.AddSingleton(x => new PaypalClient(
    builder.Configuration["PaypalOptions:AppId"],
    builder.Configuration["PaypalOptions:AppSecret"],
    builder.Configuration["PaypalOptions:Mode"]
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Thêm (Vì yêu c?u ?úng th? t? nên ph?i v?y)
app.UseSession(); //Su dung session
app.UseAuthentication(); // Xac Thuc Nguoi Dun, Dung` trong DangNhapController
// h?t

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

