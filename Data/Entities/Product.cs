
using System.ComponentModel;

namespace QuanLyBanHang.Data.Entities;

public class Product
{
    public int Id { get; set; }
    public string? ProductName { get; set; }
    public string? ImagePath { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int BranchId { get; set; }
    public Category? Category { get; set; }
}