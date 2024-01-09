using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.ViewModels.AutoMapper;
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderViewModel>();
        CreateMap<OrderViewModel, Order>();
    }
}