using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly TradeMarketDbContext _dbContext;

        public PersonRepository(TradeMarketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _dbContext.Persons.ToListAsync();
        }

        public async Task<Person> GetByIdAsync(int id)
        {
            return await _dbContext.Persons.FindAsync(id);
        }

        public async Task AddAsync(Person entity)
        {
            await _dbContext.Persons.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(Person entity)
        {
            _dbContext.Persons.Remove(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var person = await _dbContext.Persons.FindAsync(id);
            if (person != null)
            {
                _dbContext.Persons.Remove(person);
                _dbContext.SaveChanges();
            }
        }

        public void Update(Person entity)
        {
            _dbContext.Persons.Update(entity);
            _dbContext.SaveChanges();
        }
    }
}
