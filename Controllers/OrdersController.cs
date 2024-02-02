
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.Services;
using QuanLyBanHang.ViewModels;
using System.Globalization;
using System.IO;
using System.Text;

namespace QuanLyBanHang.Controllers;
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IBranchService _branchService;
    private readonly ICategoryService _categoryService;

    public OrdersController(IOrderService orderService, IBranchService branchService, ICategoryService categoryService)
    {
        _orderService = orderService;
        _branchService = branchService;
        _categoryService = categoryService;
    }
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllAsync();
        foreach (var order in orders)
        {
            var branch = await _branchService.GetAsync(order.BranchId);
            order.BranchName = branch.BranchName;
        }
        return View(orders);
    }
    public async Task<IActionResult> ThongKe()
    {
        var listOrder = _orderService.GetOrderFilter();
        ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "BranchName", "BranchName");
        ViewData["CategoryName"] = new SelectList(await _categoryService.GetAllAsync(), "CategoryName", "CategoryName");
        IEnumerable<OrderFilter> newList = listOrder as IEnumerable<OrderFilter>;
        if (newList != null)
        {
            //TempData["liOrder"] = newList;
            var json = JsonConvert.SerializeObject(newList);
            ViewData["ListOrder"] = json;
        }
        return View(newList);
    }
    [HttpPost]
    public async Task<IActionResult> ListOrder(
        string searchProductName,
        string searchBranchName,
        string searchDateCreated,
        string searchCustomerName,
        string searchCategory
        )
    {
        var listOrder = _orderService.GetOrderFilter();
        if (!string.IsNullOrEmpty(searchProductName))
            listOrder = listOrder.Where(x => x.ProductName!.Contains(searchProductName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrEmpty(searchBranchName))
            listOrder = listOrder.Where(x => x.BranchName!.Contains(searchBranchName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrEmpty(searchDateCreated))
        {
            DateTime dateToCompare = DateTime.ParseExact(searchDateCreated, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            listOrder = listOrder.Where(x => x.CreateDate.Date == dateToCompare.Date).ToList();
        }
        if (!string.IsNullOrEmpty(searchCustomerName))
            listOrder = listOrder.Where(x => x.CustomerName!.Contains(searchCustomerName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrEmpty(searchCategory))
            listOrder = listOrder.Where(x => x.CategoryName!.Contains(searchCategory, StringComparison.OrdinalIgnoreCase)).ToList();
        IEnumerable<OrderFilter> newList = listOrder as IEnumerable<OrderFilter>;
        if (newList != null)
        {
            //TempData["listOrder"] = newList;
            var json = JsonConvert.SerializeObject(newList);
            ViewData["ListOrder"] = json;
        }
        ViewData["BranchName"] = new SelectList(await _branchService.GetAllAsync(), "BranchName", "BranchName");
        ViewData["CategoryName"] = new SelectList(await _categoryService.GetAllAsync(), "CategoryName", "CategoryName");
        return PartialView("Export", newList);
    }
    [HttpPost]
    public IActionResult ReadFileCsv(string fileContent)
    {
        if (fileContent != null && fileContent.Length > 0)
        {
            using (var reader = new StringReader(fileContent))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                try
                {
                    // Đặt định dạng ngày tháng cho CsvReader
                    var orders = csv.GetRecords<OrderRequest>().ToList();
                    IEnumerable<OrderRequest> newList = orders as IEnumerable<OrderRequest>;
                    return PartialView("Import", newList);
                }
                catch(HeaderValidationException ex1)
                {
                    return Json(new { err = ex1.Message, message = "Dữ liệu trong file không hợp lệ\n" +
                        "Lưu ý file cần bao gồm:\n" +
                        "CreateDate, Payments, Transportation, Address, Status, Totals, CustomerId, BranchId"
                    });
                }
                catch(TypeConverterException ex2)
                {
                    var errorString = ex2.Message;
                    // Cắt phần text và nội dung của text
                    int textIndex = errorString.IndexOf("Text:");
                    int textIndexEnd = errorString.IndexOf("\r", textIndex);
                    string text = errorString.Substring(textIndex, (textIndexEnd - textIndex)).Trim();

                    // Cắt phần raw record và nội dung của raw record
                    int rawRecordIndex = errorString.IndexOf("RawRecord:");
                    string rawRecord = errorString.Substring(rawRecordIndex + 11).Trim();
                    return Json(new { err = ex2.Message, message = "Chuyển đổi dữ liệu thất bại\n" +
                        "Tại bản ghi: " + rawRecord + "\nNội dung: " + text
                    });
                }
                catch (Exception ex)
                {
                    return Json(new { err = ex.Message, message = "Không đọc được file" });
                }
            }
        }
        return Content("Không tìm thấy file hoặc file trống.");
    }
    [HttpPost]
    public async Task<IActionResult> AddOrder(string fileContent)
    {
        if (fileContent != null && fileContent.Length > 0)
        {
            using (var reader = new StringReader(fileContent))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                try
                {
                    var orders = csv.GetRecords<OrderRequest>().ToList();
                    var result = await _orderService.AddListAsync(orders);
                    if (result > 0)
                        return Json(new { success = true });
                    return Json(new { success = false });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, err = ex.Message });
                }
            }
        }
        return Content("Lỗi đăng ký");
    }
    public IActionResult WriteFileCsv(string jsonData)
    {
        //var jsonData = TempData["listOrder"];
            var orders = JsonConvert.DeserializeObject<IEnumerable<OrderFilter>>(jsonData);
        if(orders!.Count() > 0)
        {
            var time = DateTime.UtcNow.AddHours(7).ToString().Replace(" ", "").Replace(":", "-").Replace("/", "-");
            var fileName = "Orders_" + time + ".csv";
            string filePath = Path.Combine("D:", fileName);
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(orders);
            }
            return Json(new { success = true, message = ("Lưu trữ file thành công: " + filePath) });
        }
        return Json(new { success = false, message = "Lưu trữ thất bại" });
    }
    [HttpPost]
    public async Task<IActionResult> DuyetDon(int id)
    {
        await _orderService.UpdateStatus(id);
        return Json(new { success = true });
    }
}