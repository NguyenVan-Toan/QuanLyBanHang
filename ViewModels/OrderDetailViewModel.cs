namespace QuanLyBanHang.ViewModels;
public class OrderDetailViewModels
{
    public int OrderDetailId { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
}