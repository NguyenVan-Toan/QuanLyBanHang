
using FluentValidation;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Validations;
public class ProductViewModelValidator : AbstractValidator<ProductViewModel>
{
    public ProductViewModelValidator()
    {
        RuleFor(p => p.ProductName).NotNull().WithMessage("Product name is required")
        .WithName("Product Name")
        .MaximumLength(50).WithMessage("Maximum length is 50 characters");

        RuleFor(p => p.Price).NotNull().WithMessage("Price is required")
        .GreaterThan(0);

        RuleFor(p => p.Quantity).NotNull().WithMessage("Quantity is required")
        .GreaterThan(0);

        RuleFor(p => p.Description).NotNull().WithMessage("Description is required");

        RuleFor(p => p.CategoryId).NotNull().WithMessage("Category name is required");

    }
}
public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(p => p.ProductName).NotNull().WithMessage("Product name is required")
        .WithName("Product Name")
        .MaximumLength(50).WithMessage("Maximum length is 50 characters");

        RuleFor(p => p.Price).NotNull().WithMessage("Price is required")
        .GreaterThan(0);

        RuleFor(p => p.Quantity).NotNull().WithMessage("Quantity is required")
        .GreaterThan(0);

        RuleFor(p => p.Description).NotNull().WithMessage("Description name is required");

        RuleFor(p => p.CategoryId).NotNull().WithMessage("Category name is required");

    }
}