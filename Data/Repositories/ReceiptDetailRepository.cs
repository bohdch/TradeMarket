using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ReceiptDetailRepository : IReceiptDetailRepository
    {
        private readonly TradeMarketDbContext _dbContext;

        public ReceiptDetailRepository(TradeMarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllAsync()
        {
            return await _dbContext.ReceiptsDetails.ToListAsync();
        }

        public async Task<ReceiptDetail> GetByIdAsync(int id)
        {
            return await _dbContext.ReceiptsDetails.FindAsync(id);
        }

        public async Task<IEnumerable<ReceiptDetail>> GetAllWithDetailsAsync()
        {
            return await _dbContext.ReceiptsDetails
                .Include(rd => rd.Product)
                    .ThenInclude(p => p.Category)
                .Include(rd => rd.Receipt)
                    .ThenInclude(r => r.Customer)
                .ToListAsync();
        }

        public async Task AddAsync(ReceiptDetail entity)
        {
            await _dbContext.ReceiptsDetails.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(ReceiptDetail entity)
        {
            _dbContext.ReceiptsDetails.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var receiptDetail = await _dbContext.ReceiptsDetails.FindAsync(id);
            if (receiptDetail != null)
            {
                _dbContext.ReceiptsDetails.Remove(receiptDetail);
                await _dbContext.SaveChangesAsync();
            }
        }

        public void Update(ReceiptDetail entity)
        {
            _dbContext.ReceiptsDetails.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
