using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace ApiEcommerce.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cache = "categories-tag";

        public CategoriesController(ICategoryRepository repository, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            _repository = repository;
            _mapper = mapper;
            _outputCacheStore = outputCacheStore;
        }

        [HttpGet(Name = "GetCategoriesV2")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = (await _repository.GetCategories()).OrderBy(c => c.Name);
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategoryByIdV2")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var categoryExists = await _repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = await _repository.GetCategoryById(id);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost(Name = "CreateCategoryV2")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto is null)
            {
                return BadRequest();
            }

            var categoryExists = await _repository.CategoryExists(createCategoryDto.Name);
            if (categoryExists)
            {
                ModelState.AddModelError(nameof(createCategoryDto.Name), "La categoría ya existe");
                return ValidationProblem();
            }

            var category = _mapper.Map<Category>(createCategoryDto);
            var categoryCreated = await _repository.CreateCategory(category);
            if (!categoryCreated)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar la categoría");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            var categoryCreatedDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("GetCategoryByIdV2", new { id = categoryCreatedDto.Id }, categoryCreatedDto);
        }

        [HttpPut("{id:int}", Name = "UpdateCategoryV2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto updateCategoryDto)
        {
            if (id <= 0 || updateCategoryDto is null)
            {
                return BadRequest();
            }

            var categoryExists = await _repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            var categoryUpdated = await _repository.UpdateCategory(category);
            if (!categoryUpdated)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar la categoría");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategoryV2")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var categoryExists = await _repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = await _repository.GetCategoryById(id);
            var categoryDeleted = await _repository.DeleteCategory(category);
            if (!categoryDeleted)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la categoría");
                return StatusCode(500, ModelState);
            }

            await _outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }
    }
}
