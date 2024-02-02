using System.IO;
using System.Globalization;
using AutoMapper;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace QuanLyBanHang.Data;
public class DbInitializer
{
    private readonly QuanLyBanHangDbContext _context;
    private readonly UserManager<Account> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly string AdminRoleName = "Admin";
    private readonly string UserRoleName = "Member";
    public DbInitializer(QuanLyBanHangDbContext context,
          UserManager<Account> userManager,
          RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task Seed(IServiceProvider serviceProvider)
    {
        // Seeding role
        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = AdminRoleName,
                NormalizedName = AdminRoleName.ToUpper(),
            });
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = UserRoleName,
                NormalizedName = UserRoleName.ToUpper(),
            });
        }

        // Seeding user
        if (!_userManager.Users.Any())
        {
            var result = await _userManager.CreateAsync(new Account
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "example@gmail.com",
                Email = "example@gmail.com",
                LockoutEnabled = false,
                PhoneNumber = "0987654321",
            }, "Admin@123");
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync("example@gmail.com");
                if (user != null)
                    await _userManager.AddToRoleAsync(user, AdminRoleName);
            }
        }
        using (var context = new QuanLyBanHangDbContext(serviceProvider.GetRequiredService<DbContextOptions<QuanLyBanHangDbContext>>()))
        {
            if (!context.Categories.Any())
            {
                string path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\\FileCSVs\\Categories.csv"}";
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var mapper = serviceProvider.GetRequiredService<IMapper>();
                    var categories = csv.GetRecords<CategoryViewModel>().ToList();
                    context.Categories.AddRange(mapper.Map<IEnumerable<Category>>(categories));
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Categories ON;");
                        context.SaveChanges();
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Categories OFF;");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
            }
            else if (!context.Branch.Any())
            {
                string pathBranch = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\\FileCSVs\\Branch.csv"}";
                using (var reader = new StreamReader(pathBranch))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var branches = csv.GetRecords<Branch>().ToList();
                    // Tiếp theo, thêm các đối tượng Product vào cơ sở dữ liệu trên SQL Server
                    context.Branch.AddRange(branches);
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Branch ON;");
                        context.SaveChanges();
                        context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Branch OFF;");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }
            }
            else if (!context.Products.Any())
            {
                string pathProduct = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\\FileCSVs\\Products.csv"}";
                using (var reader = new StreamReader(pathProduct))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var mapper = serviceProvider.GetRequiredService<IMapper>();
                    var products = csv.GetRecords<ProductSeedData>().ToList();
                    // Tiếp theo, thêm các đối tượng Product vào cơ sở dữ liệu trên SQL Server
                    context.Products.AddRange(mapper.Map<IEnumerable<Product>>(products));
                    context.SaveChanges();
                }
            }
            else return;

        }
    }
}