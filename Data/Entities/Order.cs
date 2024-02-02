
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuanLyBanHang.Data.Entities;
public class Order
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
    public int BranchId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderDetail>? OrderDetail { get; set; }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasMany(o => o.OrderDetail)
        .WithOne(od => od.Order)
        .HasForeignKey(od => od.OrderId)
        .HasPrincipalKey(o => o.Id);
    }
}