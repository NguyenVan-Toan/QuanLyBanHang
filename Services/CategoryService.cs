
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Services;
public interface ICategoryService
{
    public Task<IEnumerable<CategoryViewModel>> GetAllAsync();
    public Task<CategoryViewModel> GetByIdAsync(int categoryId);
    public Task<int> CreateAsync(CategoryRequest categoryRequest);
    public Task<int> UpdateAsync(CategoryViewModel categoryViewModel);
    public Task<int> DeleteAsync(int categoryId);

}
public class CategoryService : ICategoryService
{
    private readonly QuanLyBanHangDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(QuanLyBanHangDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<int> CreateAsync(CategoryRequest categoryRequest)
    {
        var category = _mapper.Map<Category>(categoryRequest);
        _context.Categories.Add(category);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int categoryId)
    {
        var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
    {
        var categories = await _context.Categories.ToListAsync();
        return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
    }

    public async Task<CategoryViewModel> GetByIdAsync(int categoryId)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        return _mapper.Map<CategoryViewModel>(category);
    }

    public async Task<int> UpdateAsync(CategoryViewModel categoryViewModel)
    {
        var category = _context.Categories.Any(c => c.Id == categoryViewModel.Id);
        if (!category)
            throw new Exception("Category dose not exist");
        _context.Update(_mapper.Map<Category>(categoryViewModel));
        return await _context.SaveChangesAsync();
    }
}