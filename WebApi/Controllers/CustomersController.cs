using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomersController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> Get()
        {
            try
            {
                var customers = await customerService.GetAllAsync();
                return Ok(customers);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerModel>> GetById(int id)
        {
            try
            {
                var customer = await customerService.GetByIdAsync(id);
                if (customer is null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("products/{id}")]
        public async Task<ActionResult<IEnumerable<CustomerModel>>> GetByProductId(int id)
        {
            try
            {
                var customers = await customerService.GetCustomersByProductIdAsync(id);
                if (customers is null)
                {
                    return NotFound();
                }
                return Ok(customers);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<CustomerModel>> Add([FromBody] CustomerModel value)
        {
            try
            {
                await customerService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerModel>> Update(int id, [FromBody] CustomerModel value)
        {
            try
            {
                if (id != value.Id)
                {
                    return BadRequest();
                }

                await customerService.UpdateAsync(value);
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
                await customerService.DeleteAsync(id);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }
    }
}
