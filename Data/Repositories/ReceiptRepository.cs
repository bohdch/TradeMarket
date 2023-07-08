using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly TradeMarketDbContext _dbContext;

        public ReceiptRepository(TradeMarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            return await _dbContext.Receipts.ToListAsync();
        }

        public async Task<Receipt> GetByIdAsync(int id)
        {
            return await _dbContext.Receipts.FindAsync(id);
        }

        public async Task<IEnumerable<Receipt>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Receipts
                .Include(r => r.Customer)
                    .ThenInclude(c => c.Person)
                .Include(r => r.ReceiptDetails)
                    .ThenInclude(rd => rd.Product)
                        .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Receipt> GetByIdWithDetailsAsync(int id)
        {
            return await _dbContext.Receipts
                .Include(r => r.Customer)
                    .ThenInclude(c => c.Person)
                .Include(r => r.ReceiptDetails)
                    .ThenInclude(rd => rd.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Receipt entity)
        {
            await _dbContext.Receipts.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(Receipt entity)
        {
            _dbContext.Receipts.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var receipt = await _dbContext.Receipts.FindAsync(id);
            if (receipt != null)
            {
                _dbContext.Receipts.Remove(receipt);
                await _dbContext.SaveChangesAsync();
            }
        }

        public void Update(Receipt entity)
        {
            _dbContext.Receipts.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
