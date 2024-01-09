using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace QuanLyBanHang.Data.Entities;
public class Customer
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AccountId { get; set; }
    public Account? Account { get; set; }
}
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasOne(a => a.Account)
        .WithOne()
        .HasForeignKey<Customer>(c => c.AccountId);
    }
}