using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

public interface IOrderService
{
    Task<IEnumerable<OrderViewModel>> GetAllAsync();
    Task<int> UpdateStatus(int id);
}
public class OrderService : IOrderService
{
    private readonly QuanLyBanHangDbContext _context;
    private readonly IMapper _mapper;
    public OrderService(QuanLyBanHangDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
    {
        var orders = await _context.Orders.ToListAsync();
        var orderViewModel = _mapper.Map<IEnumerable<OrderViewModel>>(orders);
        foreach (var order in orderViewModel)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == order.CustomerId);
            order.CustomerName = customer!.FullName;
        }
        return orderViewModel;
    }
    public async Task<int> UpdateStatus(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        order!.Status = "Đã duyệt";
        return await _context.SaveChangesAsync();
    }
}