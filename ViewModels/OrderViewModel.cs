using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.ViewModels;
public class OrderViewModel
{
    public int Id { get; set; }
    [DataType(DataType.Date)]
    public DateTime CreateDate { get; set; }
    public string? Payments { get; set; }
    public string? Transportation { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public int Totals { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
}