
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels;
public class ProductViewModel
{
    public int Id { get; set; }

    public string? ProductName { get; set; }
    public string? ImagePath { get; set; }
    public IFormFile? Image { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
public class ProductRequest
{
    public string? ProductName { get; set; }
    public IFormFile? Image { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
public class ProductSeedData
{
    public string? ProductName { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}