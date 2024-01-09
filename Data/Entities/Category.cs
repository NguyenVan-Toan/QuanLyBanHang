
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuanLyBanHang.Data.Entities;
public class Category
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }

    public ICollection<Product>? Products { get; set; }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasMany(c => c.Products)
        .WithOne(p => p.Category)
        .HasForeignKey(p => p.CategoryId)
        .HasPrincipalKey(cate => cate.Id);
    }
}