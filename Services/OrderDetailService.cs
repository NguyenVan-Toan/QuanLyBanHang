
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Services;
public interface IOrderDetailService
{
    Task<IEnumerable<OrderDetailViewModels>> GetOrderDetailByOrder(int id);
    Task<int> Create(ProductViewModel product, string userId);
}
public class OrderDetailService : IOrderDetailService
{
    private readonly QuanLyBanHangDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly IOrderService _orderService;
    public OrderDetailService(IOrderService orderService, QuanLyBanHangDbContext context, IMapper mapper, SignInManager<Account> signInManager, UserManager<Account> userManager)
    {
        _orderService = orderService;
        _context = context;
        _mapper = mapper;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    public Task<int> Create(ProductViewModel product, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<OrderDetailViewModels>> GetOrderDetailByOrder(int orderId)
    {
        var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == orderId).ToListAsync();
        return _mapper.Map<IEnumerable<OrderDetailViewModels>>(orderDetails);
    }
}