using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsInPages(int pageNumber, int pageSize)
        {
            return await _context.Products
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public int GetTotalProducts()
        {
            return _context.Products.Count();
        }

        public async Task<IEnumerable<Product>> GetProductsForCategory(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProducts(string searchTerm)
        {
            var formatSearchTerm = formatString(searchTerm);
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => formatString(p.Name).Contains(formatSearchTerm) || formatString(p.Description).Contains(formatSearchTerm))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> BuyProduct(int id, int quantity)
        {
            var product = (await GetProductById(id))!;
            product.Stock -= quantity;
            return await UpdateProduct(product);
        }

        public async Task<bool> ProductExists(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ProductExists(string name)
        {
            return await _context.Products.AnyAsync(p => formatString(p.Name) == formatString(name));
        }

        public async Task<bool> CreateProduct(Product product) {
            _context.Products.Add(product);
            return await Save();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            return await Save();
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        private string formatString(string name) => name.ToLower().Trim();
    }
}
