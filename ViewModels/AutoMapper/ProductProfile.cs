using AutoMapper;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels.AutoMapper;
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductViewModel>();
        CreateMap<ProductViewModel, Product>();
        CreateMap<ProductRequest, Product>();
        CreateMap<ProductSeedData, Product>();
    }
}