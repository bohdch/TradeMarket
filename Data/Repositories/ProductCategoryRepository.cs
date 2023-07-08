using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly TradeMarketDbContext _dbContext;

        public ProductCategoryRepository(TradeMarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllAsync()
        {
            return await _dbContext.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory> GetByIdAsync(int id)
        {
            return await _dbContext.ProductCategories.FindAsync(id);
        }

        public async Task AddAsync(ProductCategory entity)
        {
            await _dbContext.ProductCategories.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(ProductCategory entity)
        {
            _dbContext.ProductCategories.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var category = await _dbContext.ProductCategories.FindAsync(id);
            if (category != null)
            {
                _dbContext.ProductCategories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
        }

        public void Update(ProductCategory entity)
        {
            _dbContext.ProductCategories.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
