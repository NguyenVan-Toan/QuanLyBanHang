using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanHang.Services;

namespace QuanLyBanHang.Controllers;
[Authorize(Roles = "Admin")]
public class OrderDetailsController : Controller
{
    private readonly IOrderDetailService _orderDetailService;
    private readonly IProductService _productService;

    public OrderDetailsController(IProductService productService, IOrderDetailService orderDetailService)
    {
        _productService = productService;
        _orderDetailService = orderDetailService;
    }
    public async Task<IActionResult> GetDetails(int id)
    {
        var orderDetails = await _orderDetailService.GetOrderDetailByOrder(id);
        foreach (var od in orderDetails)
        {
            var product = await _productService.GetProductByIdAsync(od.ProductId);
            od.ProductName = product.ProductName;
        }
        return View(orderDetails);
    }
}