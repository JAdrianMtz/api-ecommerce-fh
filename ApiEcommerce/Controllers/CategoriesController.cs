using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository repository;
        private readonly IMapper mapper;

        public CategoriesController(ICategoryRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet(Name = "GetCategories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = repository.GetCategories();
            var categoriesDto = mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CategoryDto> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var categoryExists = repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = repository.GetCategoryById(id);
            var categoryDto = mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost(Name = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CategoryDto> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto is null)
            {
                return BadRequest();
            }

            var categoryExists = repository.CategoryExists(createCategoryDto.Name);
            if (categoryExists)
            {
                ModelState.AddModelError(nameof(createCategoryDto.Name), "La categoría ya existe");
                return ValidationProblem();
            }

            var category = mapper.Map<Category>(createCategoryDto);
            var categoryCreated = repository.CreateCategory(category);
            if (!categoryCreated)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar la categoría");
                return StatusCode(500, ModelState);
            }

            var categoryCreatedDto = mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("GetCategoryById", new { id = categoryCreatedDto.Id }, categoryCreatedDto);
        }

        [HttpPut("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, CreateCategoryDto updateCategoryDto)
        {
            if (id <= 0 || updateCategoryDto is null)
            {
                return BadRequest();
            }

            var categoryExists = repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = mapper.Map<Category>(updateCategoryDto);
            category.Id = id;
            var categoryUpdated = repository.UpdateCategory(category);
            if (!categoryUpdated)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar la categoría");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var categoryExists = repository.CategoryExists(id);
            if (!categoryExists)
            {
                return NotFound();
            }

            var category = repository.GetCategoryById(id);
            var categoryDeleted = repository.DeleteCategory(category);
            if (!categoryDeleted)
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la categoría");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
