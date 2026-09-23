using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProductsInPages(int pageNumber, int pageSize);
        int GetTotalProducts();
        Task<IEnumerable<Product>> GetProductsForCategory(int categoryId);
        Task<IEnumerable<Product>> SearchProducts(string searchTerm);
        Task<Product?> GetProductById(int id);
        Task<bool> BuyProduct(int id, int quantity);
        Task<bool> ProductExists(int id);
        Task<bool> ProductExists(string name);
        Task<bool> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(Product product);
        Task<bool> Save();
    }
}
