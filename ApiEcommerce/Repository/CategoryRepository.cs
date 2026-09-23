using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> CategoryExists(string name)
        {
            return await _context.Categories.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task<bool> CreateCategory(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(category);
            return await Save();
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            return await Save();
        }

        public async Task<bool> DeleteCategory(Category category)
        {
            _context.Categories.Remove(category);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
