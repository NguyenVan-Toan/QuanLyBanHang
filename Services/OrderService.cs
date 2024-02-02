using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

public interface IOrderService
{
    Task<IEnumerable<OrderViewModel>> GetAllAsync();
    Task<int> UpdateStatus(int id);
    List<OrderFilter> GetOrderFilter();
    Task<int> AddListAsync(List<OrderRequest> orderRequest);
}
public class OrderService : IOrderService
{
    private readonly QuanLyBanHangDbContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;
    public OrderService(QuanLyBanHangDbContext context, IMapper mapper, IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    public async Task<int> AddListAsync(List<OrderRequest> orderRequest)
    {
        var orders = _mapper.Map<List<Order>>(orderRequest);
        var newList = orders as IEnumerable<Order>;
        _context.Orders.AddRange(newList);
        return await _context.SaveChangesAsync();
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

    public List<OrderFilter> GetOrderFilter()
    {
        var result = _context.Orders
            .Join(
                _context.OrderDetails,
                order => order.Id,
                orderDetails => orderDetails.OrderId,
                (order,orderDetails) => new {order, orderDetails})
            .Join(
                _context.Products,
                orderOrderDetails => orderOrderDetails.orderDetails.ProductId,
                product => product.Id,
                (orderOrderDetails, product) => new {orderOrderDetails.order, orderOrderDetails.orderDetails, product})
            .Join(
                _context.Customers,
                data => data.order.CustomerId,
                customer => customer.Id,
                (data, customer) => new { data.order, data.orderDetails, data.product, customer }
            )
            .Join(
                _context.Branch,
                data => data.order.BranchId,
                branch => branch.Id,
                (data, branch) => new { data.order, data.orderDetails, data.product, data.customer, branch }
            )
            .Join(
                _context.Categories,
                data => data.product.CategoryId,
                category => category.Id,
                (data, category) => new { data.order, data.orderDetails, data.product, data.customer, data.branch, category}
            )
            .Select(data => new 
            {
                ProductName = data.product.ProductName,
                CreateDate = data.order.CreateDate,
                CustomerName = data.customer.FullName,
                BranchName = data.branch.BranchName,
                OrderId = data.order.Id,
                Number = data.orderDetails.Quantity,
                Status = data.order.Status,
                Totals = data.orderDetails.Quantity * data.orderDetails.Price,
                CategoryName = data.category.CategoryName
            });
        List<OrderFilter> listOrder = new List<OrderFilter>();
        foreach (var item in result)
        {
            var e = new OrderFilter();
            e.CreateDate = item.CreateDate;
            e.OrderId = item.OrderId;
            e.Number = item.Number;
            e.Totals = item.Totals;
            e.Status = item.Status;
            e.BranchName = item.BranchName;
            e.ProductName = item.ProductName;
            e.CustomerName = item.CustomerName;
            e.CategoryName = item.CategoryName;
            listOrder.Add(e);

        }
        return listOrder;
    }

    public async Task<int> UpdateStatus(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        order!.Status = "Đã duyệt";
        return await _context.SaveChangesAsync();
    }
}