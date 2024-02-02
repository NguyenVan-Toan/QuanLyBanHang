namespace QuanLyBanHang.ViewModels;
public class CartViewModel
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Price { get; set; }
    public int Number { get; set; }
    public int Totals { get; set; }
    public string? Branch { get; set; }
}