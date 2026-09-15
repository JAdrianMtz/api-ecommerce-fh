using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetProductsForCategory(int categoryId);
        IEnumerable<Product> SearchProducts(string searchTerm);
        Product? GetProductById(int id);
        bool BuyProduct(int id, int quantity);
        bool ProductExists(int id);
        bool ProductExists(string name);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Product product);
    }
}
