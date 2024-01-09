using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.Services;
public interface ICartService
{
    Task<int> Create(int productId, string userId);
    Task<IEnumerable<Cart>> GetCartsByCustomer(string userId);
    Task<Cart> GetCartsById(int id);
    Task<int> DeleteCartsByCustomer(int customerId);
    Task<int> UpdateCartByIdAsync(int id, int num);
}
public class CartService : ICartService
{
    private readonly QuanLyBanHangDbContext _context;
    public CartService(QuanLyBanHangDbContext context)
    {
        _context = context;
    }
    public async Task<int> Create(int productId, string userId)
    {
        var customerId = _context.Customers.FirstOrDefault(c => c.AccountId == userId)!.Id;
        var cart = _context.Carts.FirstOrDefault(c => c.CustomerId == customerId && c.ProductId == productId);
        if (cart != null)
        {
            cart.Number++;
        }
        else
        {
            Cart newCart = new Cart { ProductId = productId, CustomerId = customerId, Number = 1 };
            await _context.Carts.AddAsync(newCart);
        }
        return await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Cart>> GetCartsByCustomer(string userId)
    {
        var customerId = _context.Customers.Where(c => c.AccountId == userId).FirstOrDefault()!.Id;
        var carts = await _context.Carts.Where(c => c.CustomerId == customerId).ToListAsync();
        return carts;
    }
    public async Task<Cart> GetCartsById(int id)
    {
        var cart = await _context.Carts.Where(c => c.Id == id).FirstOrDefaultAsync();
        return cart!;
    }
    public async Task<int> DeleteCartsByCustomer(int customerId)
    {
        var carts = _context.Carts.Where(c => c.CustomerId == customerId).ToList();
        foreach (var cart in carts)
        {
            _context.Carts.Remove(cart);
        }
        return await _context.SaveChangesAsync();

    }
    public async Task<int> UpdateCartByIdAsync(int id, int num)
    {
        var cart = await GetCartsById(id);
        cart.Number += num;
        if (cart.Number > 0)
        {
            var entry = _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return entry.Entity.Number;
        }
        else
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return 0;
        }
    }
}