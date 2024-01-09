
using FluentValidation;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Validations;
public class CategoryViewModelValidator : AbstractValidator<CategoryViewModel>
{
    public CategoryViewModelValidator()
    {
        RuleFor(c => c.CategoryName).NotEmpty().WithMessage("The category name is required")
        .WithName("Category Name");

        RuleFor(c => c.Description).NotNull().WithMessage("The description is required");
    }
}
public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(c => c.CategoryName).NotEmpty().WithMessage("The category name is required")
        .WithName("Category Name");

        RuleFor(c => c.Description).NotNull().WithMessage("The description is required");
    }
}