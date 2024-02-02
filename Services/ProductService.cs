
using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;
using QuanLyBanHang.ViewModels;

namespace QuanLyBanHang.Services;
public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
    Task<ProductViewModel> GetProductByIdAsync(int id);
    Task<int> Create(ProductRequest productRequest);
    Task<int> Update(ProductViewModel productViewModel);
    Task<int> Delete(int id);

}
public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly QuanLyBanHangDbContext _context;
    private readonly IStorageService _storageService;
    private const string USER_CONTENT_FOLDER_NAME = "user-content";
    public ProductService(
        IMapper mapper, 
        QuanLyBanHangDbContext context, 
        IStorageService storageService
        )
    {
        _mapper = mapper;
        _context = context;
        _storageService = storageService;
    }
    private async Task<string> SaveFile(IFormFile file)
    {
        var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName!.Trim('"');
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
        return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;
    }
    public async Task<int> Create(ProductRequest productRequest)
    {
        var product = _mapper.Map<Product>(productRequest);
        // Save image file
        if (productRequest.Image != null)
        {
            product.ImagePath = await SaveFile(productRequest.Image);
        }
        _context.Add(product);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            if (!string.IsNullOrEmpty(product.ImagePath))
                await _storageService.DeleteFileAsync(product.ImagePath.Replace("/" + USER_CONTENT_FOLDER_NAME + "/", ""));
            _context.Products.Remove(product);
        }
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return _mapper.Map<IEnumerable<ProductViewModel>>(products);
    }

    public async Task<ProductViewModel> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        return _mapper.Map<ProductViewModel>(product);
    }

    public async Task<int> Update(ProductViewModel productViewModel)
    {
        var productVM = _context.Products.Any(p => p.Id == productViewModel.Id);
        if (!productVM)
            throw new Exception("Product dose not exist");
        // Save image file
        if (productViewModel.Image != null)
        {
            if (!string.IsNullOrEmpty(productViewModel.ImagePath))
                await _storageService.DeleteFileAsync(productViewModel.ImagePath.Replace("/" + USER_CONTENT_FOLDER_NAME + "/", ""));
            productViewModel.ImagePath = await SaveFile(productViewModel.Image);
        }

        var product = _mapper.Map<Product>(productViewModel);
        _context.Update(product);
        return await _context.SaveChangesAsync();
    }
}