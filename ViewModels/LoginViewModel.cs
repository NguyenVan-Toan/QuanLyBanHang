
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanHang.ViewModels;
public class LoginViewModel
{
    [Required]
    [Display(Name = "User name or email")]
    public string? UserNameOrEmail { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}