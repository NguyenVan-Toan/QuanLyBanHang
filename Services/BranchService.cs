using Microsoft.EntityFrameworkCore;
using QuanLyBanHang.Data;
using QuanLyBanHang.Data.Entities;

namespace QuanLyBanHang.Services
{
    public interface IBranchService
    {
        public Task<List<Branch>> GetAllAsync();
        public Task<Branch> GetAsync(int id);
    }
    public class BranchService : IBranchService
    {
        private readonly QuanLyBanHangDbContext _context;
        public BranchService(QuanLyBanHangDbContext context)
        {
            _context = context;
        }
        public async Task<List<Branch>> GetAllAsync()
        {
            var branches = await _context.Branch.ToListAsync();
            return branches;
        }

        public async Task<Branch> GetAsync(int id)
        {
            var branch = await _context.Branch.FindAsync(id);
            return branch != null ? branch : null!;
        }
    }
}
