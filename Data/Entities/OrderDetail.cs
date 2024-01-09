
// using System.Data.Odbc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuanLyBanHang.Data.Entities;
public class OrderDetail
{
    public int OrderDetailId { get; set; }
    public int Price { get; set; }
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

}
public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(od => od.OrderDetailId);

        // builder.HasOne(od => od.Product)
        //     .WithOne()
        //     .HasForeignKey<OrderDetail>(od => od.ProductId);
    }
}
