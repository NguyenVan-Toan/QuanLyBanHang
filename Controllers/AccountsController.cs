
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Controllers;
[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly QuanLyBanHangDbContext _context;
    // private readonly IEmailSender _emailSender;
    private readonly ILogger<AccountController> _logger;
    // IEmailSender emailSender,
    public AccountController(
            UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            QuanLyBanHangDbContext context,
            ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        // _emailSender = emailSender;
        _logger = logger;
        _context = context;
    }
    [HttpGet("/login/")]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = null!)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    // POST: /Account/Login
    [HttpPost("/login/")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null!)
    {
        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserNameOrEmail!, model.Password!, model.RememberMe, lockoutOnFailure: true);
            // Tìm UserName theo Email, đăng nhập lại
            if (!result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.UserNameOrEmail!);
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, lockoutOnFailure: true);
                }
            }

            if (result.Succeeded)
            {
                _logger.LogInformation(1, "User logged in.");
                return LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning(2, "Tài khoản bị khóa");
                return View("Lockout");
            }
            else
            {
                // ModelState.AddModelError("Không đăng nhập được.");
                ViewData["ErrorLogin"] = "Sai tai khoan hoac mat khau";
                return View(model);
            }
        }
        return View(model);
    }
    // POST: /Account/LogOff
    [HttpPost("/logout/")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User đăng xuất");
        return RedirectToAction("Index", "Home", new { area = "" });
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string returnUrl = null!)
    {
        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null!)
    {
        returnUrl ??= Url.Content("~/");
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = new Account { UserName = model.UserName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password!);
            // Add role to the account
            await _userManager.AddToRoleAsync(user, "Member");
            // Add new customer
            var customer = new Customer { FullName = "", Email = model.Email, Address = "", AccountId = user.Id };
            _context.Customers.Add(customer);
            _context.SaveChanges();
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }
        }

        // If we got this far, something failed, redisplay form
        ViewData["ErrorRegister"] = "Tai khoan da ton tai";
        return View(model);
    }
    // [Route("/khongduoctruycap.html")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}