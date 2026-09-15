using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository repository;
        private readonly IMapper mapper;
        private readonly ICategoryRepository categoryRepository;

        public ProductsController(IProductRepository repository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.repository = repository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            var products = repository.GetProducts();
            var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("by-category", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ProductDto>> GetProductsForCategory([FromQuery] int categoryId) { 
            if (categoryId <= 0)
            {
                return BadRequest();
            }

            var products = repository.GetProductsForCategory(categoryId);
            var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("search", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ProductDto>> SearchProducts([FromQuery] string searchTerm) { 
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest();
            }

            var products = repository.SearchProducts(searchTerm);
            var productsDto = mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductDto> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var productExists = repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = repository.GetProductById(id);
            var productDto = mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost("{productId:int}/buy", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuyProduct(int productId, int quantity)
        {
            if (productId <= 0)
            {
                return BadRequest();
            }

            if (quantity <= 0)
            {
                ModelState.AddModelError(nameof(quantity), "El valor debe ser mayor que 0");
                return ValidationProblem();
            }

            var productExists = repository.ProductExists(productId);
            if (!productExists)
            {
                return NotFound();
            }

            var product = repository.GetProductById(productId)!;
            if (quantity > product.Stock)
            {
                ModelState.AddModelError(string.Empty, "No hay suficiente stock del producto");
                return ValidationProblem();
            }

            var success = repository.BuyProduct(productId, quantity);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Error al comprar el producto");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ProductDto> CreateProduct(CreateProductDto createProductDto)
        {
            if (createProductDto is null)
            {
                return BadRequest();
            }

            var productExists = repository.ProductExists(createProductDto.Name);
            if (productExists)
            {
                ModelState.AddModelError(string.Empty, "El producto ya existe");
                return ValidationProblem();
            }

            var categoryExists = categoryRepository.CategoryExists(createProductDto.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(createProductDto.CategoryId), "La categoría no existe");
                return ValidationProblem();
            }

            var product = mapper.Map<Product>(createProductDto);
            var productCreated = repository.CreateProduct(product);
            if (!productCreated)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar el producto");
                return StatusCode(500, ModelState);
            }

            var productDto = mapper.Map<ProductDto>(product);
            return CreatedAtRoute("GetProductById", new { id = productDto.Id }, productDto);
        }

        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            if (id <= 0 || updateProductDto is null)
            {
                return BadRequest();
            }

            var productExists = repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var categoryExists = categoryRepository.CategoryExists(updateProductDto.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(updateProductDto.CategoryId), "La categoría no existe");
                return ValidationProblem();
            }

            var product = mapper.Map<Product>(updateProductDto);
            product.Id = id;
            var updatedProduct = repository.UpdateProduct(product);
            if (!updatedProduct)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el producto");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var productExists = repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = repository.GetProductById(id)!;
            var deletedProduct = repository.DeleteProduct(product);
            if (!deletedProduct)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar el producto");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
