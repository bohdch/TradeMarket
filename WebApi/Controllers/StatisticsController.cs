using System;
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
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticService statisticService;

        public StatisticsController(IStatisticService statisticService)
        {
            this.statisticService = statisticService;
        }

        [HttpGet("popularProducts")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetMostPopularProducts([FromQuery] int productCount)
        {
            try
            {
                var products = await statisticService.GetMostPopularProductsAsync(productCount);
                return Ok(products);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("customer/{id}/{productCount}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetCustomersMostPopularProducts(int id, int productCount)
        {
            try
            {
                var products = await statisticService.GetCustomersMostPopularProductsAsync(productCount, id);
                return Ok(products);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("activity/{customerCount}")]
        public async Task<ActionResult<IEnumerable<CustomerActivityModel>>> GetMostValuableCustomers(int customerCount,
            [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var customers = await statisticService.GetMostValuableCustomersAsync(customerCount, startDate, endDate);
                return Ok(customers);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("income/{categoryId}")]
        public async Task<ActionResult<decimal>> GetIncomeOfCategoryInPeriod(int categoryId,
             [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var income = await statisticService.GetIncomeOfCategoryInPeriod(categoryId, startDate, endDate);
                return Ok(income);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }
    }
}
