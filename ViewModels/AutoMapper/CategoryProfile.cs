using AutoMapper;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels.AutoMapper;
public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryViewModel, Category>();
        CreateMap<CategoryRequest, Category>();
    }
}