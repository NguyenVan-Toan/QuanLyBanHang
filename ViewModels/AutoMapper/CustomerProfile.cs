using AutoMapper;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels.AutoMapper;
public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<CustomerRequest, Customer>();
    }
}