using ApiEcommerce.Models;

namespace ApiEcommerce.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<Category?> GetCategoryById(int id);
        Task<bool> CategoryExists(int id);
        Task<bool> CategoryExists(string name);
        Task<bool> CreateCategory(Category category);
        Task<bool> UpdateCategory(Category category);
        Task<bool> DeleteCategory(Category category);
        Task<bool> Save();
    }
}
