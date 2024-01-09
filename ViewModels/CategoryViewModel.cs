using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels;
public class CategoryViewModel
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
}
public class CategoryRequest
{
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
}