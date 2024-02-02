using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.ViewModels;
public class OrderViewModel
{
    public int Id { get; set; }
    [DataType(DataType.Date)]
    [Display(Name = "Create Date")]
    public DateTime CreateDate { get; set; }
    public string? Payments { get; set; }
    public string? Transportation { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public int Totals { get; set; }
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
    [Display(Name ="Branch")]
    public string? BranchName {  get; set; }
    [Display(Name = "Customer Name")]
    public string? CustomerName { get; set; }
}
public class OrderRequest
{
    [DataType(DataType.Date)]
    [Display(Name = "Create Date")]
    [Format("dd/MM/yyyy")]
    public DateTime CreateDate { get; set; }
    public string? Payments { get; set; }
    public string? Transportation { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public int Totals { get; set; }
    public int CustomerId { get; set; }
    public int BranchId { get; set; }
}
public class OrderFilter
{
    public int OrderId { get; set; }
    [DataType(DataType.Date)]
    [Display(Name = "Create Date")]
    public DateTime CreateDate { get; set; }
    [Display(Name = "Customer Name")]
    public string? CustomerName { get; set; }
    [Display(Name = "Product Name")]
    public string? ProductName { get; set; }
    [Display(Name = "Branch")]
    public string? BranchName { get; set; }
    public int Number {  get; set; }
    public string? Status { get; set; }
    public int Totals { get; set; }
    [Display(Name = "Category Name")]
    public string? CategoryName { get; set; }

}