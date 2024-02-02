
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.Services;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Controllers;
public class CartsController : Controller
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly QuanLyBanHangDbContext _context;
    private readonly ICartService _cartService;
    private readonly IEmailSender _emailSender;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    public CartsController(IMapper mapper, IProductService productService, ICartService cartService, IEmailSender emailSender, SignInManager<Account> signInManager, UserManager<Account> userManager, QuanLyBanHangDbContext context)
    {
        _mapper = mapper;
        _productService = productService;
        _cartService = cartService;
        _emailSender = emailSender;
        _userManager = userManager;
        _context = context;
        _signInManager = signInManager;

    }
    [Route("/xem-gio-hang")]
    public async Task<IActionResult> ShowCart()
    {
        if (_signInManager.IsSignedIn(User))
        {
            var userId = _userManager.GetUserId(User);
            var carts = await _cartService.GetCartsByCustomer(userId!);
            var listCarts = new List<CartViewModel>();
            foreach (var c in carts)
            {
                var product = await _productService.GetProductByIdAsync(c.ProductId);
                CartViewModel cartViewModel = new CartViewModel { Id = c.Id, ProductName = product.ProductName, Price = product.Price, ProductId = c.ProductId, Number = c.Number, Totals = product.Price * c.Number };
                listCarts.Add(cartViewModel);
            }
            return View(listCarts);
        }
        else return View(nameof(XuLyDangNhap));

    }
    public IActionResult XuLyDangNhap()
    {
        return View();
    }
    public async Task<IActionResult> ThanhToan(string id, string ship)
    {
        int branchId = 1;
        if (ship == "2")
            branchId = 2;
        var carts = await _cartService.GetCartsByCustomer(id);
        var customer = _context.Customers.FirstOrDefault(c => c.AccountId == id);
        Order order = new Order { 
            CreateDate = DateTime.Now, 
            Payments = "Thanh toan truc tiep", 
            Transportation = "Van chuyen nhanh", 
            Address = customer!.Address, 
            BranchId = branchId, 
            Status = "Chờ duyệt", 
            Totals = 0, 
            Customer = customer };
        var entryOrder = _context.Orders.Add(order);
        _context.SaveChanges();
        var orderId = entryOrder.Entity.Id;
        var totals = 0;
        string listProducts = "";
        foreach (var cart in carts)
        {
            var product = await _productService.GetProductByIdAsync(cart.ProductId);
            OrderDetail orderDetail = new OrderDetail { Price = product.Price, Quantity = cart.Number, OrderId = orderId, ProductId = product.Id };
            totals += cart.Number * product.Price;
            listProducts += $"<p>Sản phẩm: {product.ProductName}, Giá tiền: {product.Price}VND, Số lượng: {cart.Number}</p>";
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }
        var fixOrder = _context.Orders.FirstOrDefault(od => od.Id == orderId);
        fixOrder!.Totals = totals;
        _context.SaveChanges();
        var customerId = order!.CustomerId;
        var email = _context.Customers.FirstOrDefault(c => c.Id == customerId)!.Email;
        await _emailSender.SendEmailAsync(
            email!,
            "Thông báo mua hàng",
            "<h3>Thông báo bạn đã mua hàng thành công</h3>"
            + listProducts
            + "<p>Tổng số tiền cần thanh toán: " + totals + "VND."
            );
        await _cartService.DeleteCartsByCustomer(customerId);
        return View("ThanhToanThanhCong");
    }
    public IActionResult ThanhToanThanhCong()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> SuaSoLuong(int id, int num)
    {
        var number = await _cartService.UpdateCartByIdAsync(id, num);
        return Json(new { success = true, newNumber = number });
    }
    // public IActionResult XuLyThongTinUser()
    // {
    //     return View();
    // }
    // [HttpPost]
    // public IActionResult XuLyThongTinUser(CustomerRequest customerRequest)
    // {
    //     var customer = _mapper.Map<Customer>(customerRequest);

    // }
}