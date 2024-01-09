
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Controllers;
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllAsync();
        return View(orders);
    }
    [HttpPost]
    public async Task<IActionResult> DuyetDon(int id)
    {
        await _orderService.UpdateStatus(id);
        return Json(new { success = true });
    }
}