using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetAll()
        {
            try
            {
                var products = await productService.GetAllAsync();
                return Ok(products);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetById(int id)
        {
            try
            {
                var product = await productService.GetByIdAsync(id);
                if (product is null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetByFilter([FromQuery] FilterSearchModel request)
        {
            try
            {
                var products = await productService.GetByFilterAsync(request);
                if (products is null)
                {
                    return NotFound();
                }
                return Ok(products);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategoryModel>>> GetAllCategories()
        {
            try
            {
                var categories = await productService.GetAllProductCategoriesAsync();
                if (categories is null)
                {
                    return NotFound();
                }
                return Ok(categories);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductModel>> Add([FromBody] ProductModel value)
        {
            try
            {
                await productService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPost("categories")]
        public async Task<ActionResult<ProductCategoryModel>> AddCategory([FromBody] ProductCategoryModel value)
        {
            try
            {
                await productService.AddCategoryAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductModel>> Update(int id, [FromBody] ProductModel value)
        {
            try
            {
                if (id != value.Id)
                {
                    return BadRequest();
                }

                await productService.UpdateAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("categories/{id}")]
        public async Task<ActionResult<ProductCategoryModel>> UpdateCategory(int id, [FromBody] ProductCategoryModel value)
        {
            try
            {
                if (id != value.Id)
                {
                    return BadRequest();
                }

                await productService.UpdateCategoryAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await productService.DeleteAsync(id);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpDelete("categories/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                await productService.RemoveCategoryAsync(id);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }
    }
}
