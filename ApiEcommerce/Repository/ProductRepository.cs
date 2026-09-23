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

        public IEnumerable<Product> GetProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Name).ToList();
        }

        public IEnumerable<Product> GetProductsInPages(int pageNumber, int pageSize)
        {
            return _context.Products
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalProducts()
        {
            return _context.Products.Count();
        }

        public IEnumerable<Product> GetProductsForCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name).ToList();
        }

        public IEnumerable<Product> SearchProducts(string searchTerm)
        {
            var formatSearchTerm = formatString(searchTerm);
            return _context.Products
                .Include(p => p.Category)
                .Where(p => formatString(p.Name).Contains(formatSearchTerm) || formatString(p.Description).Contains(formatSearchTerm))
                .OrderBy(p => p.Name).ToList();
        }

        public Product? GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public bool BuyProduct(int id, int quantity)
        {
            var product = GetProductById(id)!;
            product.Stock -= quantity;
            return UpdateProduct(product);
        }

        public bool ProductExists(int id)
        {
            return _context.Products.Any(p => p.Id == id);
        }

        public bool ProductExists(string name)
        {
            return _context.Products.Any(p => formatString(p.Name) == formatString(name));
        }

        public bool CreateProduct(Product product) {
            _context.Products.Add(product);
            return Save();
        }

        public bool UpdateProduct(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            return Save();
        }

        public bool DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            return Save();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }

        private string formatString(string name) => name.ToLower().Trim();
    }
}
