
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace QuanLyBanHang.Data.Entities;
public class Account : IdentityUser
{
    // public override string? UserName { get; set; }
    // public string? Password { get; set; }

}
// public class AccountConfiguration : IEntityTypeConfiguration<Account>
// {
//     public void Configure(EntityTypeBuilder<Account> builder)
//     {
//         builder.HasKey(e => e.UserName);
//     }
// }