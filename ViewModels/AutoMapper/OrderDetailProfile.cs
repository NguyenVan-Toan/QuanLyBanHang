using AutoMapper;
using QuanLyBanHang.Data.Entities;
namespace QuanLyBanHang.ViewModels.AutoMapper;
public class OrderDetailProfile : Profile
{
    public OrderDetailProfile()
    {
        CreateMap<OrderDetail, OrderDetailViewModels>();
    }
}