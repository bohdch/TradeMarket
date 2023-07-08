using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TradeMarketDbContext _dbContext;

        public CustomerRepository(TradeMarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Customers
                .Include(c => c.Person)
                .Include(c => c.Receipts)
                    .ThenInclude(r => r.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            return await _dbContext.Customers.Include(c => c.Person)
                                             .Include(c => c.Receipts)
                                             .ThenInclude(r => r.ReceiptDetails)
                                       .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Customer entity)
        {
            await _dbContext.Customers.AddAsync(entity);
        }

        public void Delete(Customer entity)
        {
            _dbContext.Customers.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                _dbContext.Customers.Remove(customer);
                _dbContext.SaveChanges();
            }
        }

        public void Update(Customer entity)
        {
            _dbContext.Customers.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
