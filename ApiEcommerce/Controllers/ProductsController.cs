using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cache = "products-tag";

        public ProductsController(IProductRepository repository, ICategoryRepository categoryRepository, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _outputCacheStore = outputCacheStore;
        }

        [HttpGet(Name = "GetProducts")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            var products = _repository.GetProducts();
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("by-category", Name = "GetProductsForCategory")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ProductDto>> GetProductsForCategory([FromQuery] int categoryId) { 
            if (categoryId <= 0)
            {
                return BadRequest();
            }

            var products = _repository.GetProductsForCategory(categoryId);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("search", Name = "SearchProducts")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<ProductDto>> SearchProducts([FromQuery] string searchTerm) { 
            if (string.IsNullOrEmpty(searchTerm))
            {
                return BadRequest();
            }

            var products = _repository.SearchProducts(searchTerm);
            var productsDto = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ProductDto> GetProductById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var productExists = _repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = _repository.GetProductById(id);
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost("{productId:int}/buy", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuyProduct(int productId, int quantity)
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

            var productExists = _repository.ProductExists(productId);
            if (!productExists)
            {
                return NotFound();
            }

            var product = _repository.GetProductById(productId)!;
            if (quantity > product.Stock)
            {
                ModelState.AddModelError(string.Empty, "No hay suficiente stock del producto");
                return ValidationProblem();
            }

            var success = _repository.BuyProduct(productId, quantity);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Error al comprar el producto");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }

        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            if (createProductDto is null)
            {
                return BadRequest();
            }

            var productExists = _repository.ProductExists(createProductDto.Name);
            if (productExists)
            {
                ModelState.AddModelError(string.Empty, "El producto ya existe");
                return ValidationProblem();
            }

            var categoryExists = _categoryRepository.CategoryExists(createProductDto.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(createProductDto.CategoryId), "La categoría no existe");
                return ValidationProblem();
            }

            var product = _mapper.Map<Product>(createProductDto);
            var productCreated = _repository.CreateProduct(product);
            if (!productCreated)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar el producto");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            var productDto = _mapper.Map<ProductDto>(product);
            return CreatedAtRoute("GetProductById", new { id = productDto.Id }, productDto);
        }

        [HttpPut("{id:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            if (id <= 0 || updateProductDto is null)
            {
                return BadRequest();
            }

            var productExists = _repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var categoryExists = _categoryRepository.CategoryExists(updateProductDto.CategoryId);
            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(updateProductDto.CategoryId), "La categoría no existe");
                return ValidationProblem();
            }

            var product = _mapper.Map<Product>(updateProductDto);
            product.Id = id;
            var updatedProduct = _repository.UpdateProduct(product);
            if (!updatedProduct)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el producto");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var productExists = _repository.ProductExists(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = _repository.GetProductById(id)!;
            var deletedProduct = _repository.DeleteProduct(product);
            if (!deletedProduct)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar el producto");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }
    }
}
