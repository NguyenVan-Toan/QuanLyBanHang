using AutoMapper;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels.AutoMapper;
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderViewModel>();
        CreateMap<OrderViewModel, Order>();
        CreateMap<OrderRequest, Order>();
    }
}