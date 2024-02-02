using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.Services;
using QuanLyBanHang.Validations;
using QuanLyBanHang.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<QuanLyBanHangDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyBanHang") ?? throw new InvalidOperationException("Connection string 'QuanLyBanHangDbContext' not found.")));
// Add Identity service
builder.Services.AddIdentity<Account, IdentityRole>()
        .AddEntityFrameworkStores<QuanLyBanHangDbContext>()
        .AddDefaultTokenProviders();
// Configure for user
builder.Services.Configure<IdentityOptions>(
    options =>
    {
        // Thiết lập về Password
        options.Password.RequireDigit = false; // Không bắt phải có số
        options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
        options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
        options.Password.RequireUppercase = false; // Không bắt buộc chữ in
        options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
        options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

        // Cấu hình Lockout - khóa user
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
        options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
        options.Lockout.AllowedForNewUsers = true;

        // Cấu hình về User.
        options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;  // Email là duy nhất

        // Cấu hình đăng nhập.
        options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
        options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

    }
);
// Add AutoMapper config to the container
builder.Services.AddAutoMapper(typeof(Program));
// Add DI to the container
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IOrderDetailService, OrderDetailService>();
builder.Services.AddTransient<IBranchService, BranchService>();
//File Storage
builder.Services.AddTransient<IStorageService, FileStorageService>();
//DbInitializer
builder.Services.AddTransient<DbInitializer>();
builder.Services.AddTransient<ICartService, CartService>();

builder.Services.AddControllersWithViews();
// Add validator for category service
builder.Services.AddValidatorsFromAssemblyContaining<CategoryRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryViewModelValidator>();
// Add validator for product service
builder.Services.AddValidatorsFromAssemblyContaining<ProductViewModelValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequestValidator>();

// Add SendMailService
builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);

builder.Services.AddTransient<IEmailSender, SendMailService>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // DbInitializer.Seed(services);
    var dbInitializer = services.GetService<DbInitializer>();
    if (dbInitializer != null)
        dbInitializer.Seed(services)
                     .Wait();
}
app.Run();
