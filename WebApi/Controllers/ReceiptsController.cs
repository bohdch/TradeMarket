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
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            this.receiptService = receiptService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetAll()
        {
            try
            {
                var receipts = await receiptService.GetAllAsync();
                return Ok(receipts);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptModel>> GetById(int id)
        {
            try
            {
                var receipt = await receiptService.GetByIdAsync(id);
                if (receipt is null)
                {
                    return NotFound();
                }
                return Ok(receipt);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<ReceiptModel>>> GetReceiptsByPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var receipts = await receiptService.GetReceiptsByPeriodAsync(startDate, endDate);
                if (receipts is null)
                {
                    return NotFound();
                }
                return Ok(receipts);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/sum")]
        public async Task<ActionResult<decimal>> GetSum(int id)
        {
            try
            {
                var sum = await receiptService.ToPayAsync(id);
                return Ok(sum);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<IEnumerable<ReceiptDetailModel>>> GetReceiptDetails(int id)
        {
            try
            {
                var receiptDetails = await receiptService.GetReceiptDetailsAsync(id);
                if (receiptDetails is null)
                {
                    return NotFound();
                }
                return Ok(receiptDetails);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ReceiptModel value)
        {
            try
            {
                await receiptService.AddAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] ReceiptModel value)
        {
            try
            {
                if (id != value.Id)
                {
                    return BadRequest();
                }

                await receiptService.UpdateAsync(value);
                return Ok(value);
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}/products/add/{productId}/{quantity}")]
        public async Task<ActionResult> AddProduct(int id, int productId, int quantity)
        {
            try
            {
                await receiptService.AddProductAsync(productId, id, quantity);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}/products/remove/{productId}/{quantity}")]
        public async Task<ActionResult> RemoveProduct(int id, int productId, int quantity)
        {
            try
            {
                await receiptService.RemoveProductAsync(productId, id, quantity);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}/checkout")]
        public async Task<ActionResult> CheckOut(int id)
        {
            try
            {
                await receiptService.CheckOutAsync(id);
                return Ok();
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
                await receiptService.DeleteAsync(id);
                return Ok();
            }
            catch (MarketException)
            {
                return BadRequest();
            }
        }
    }
}
