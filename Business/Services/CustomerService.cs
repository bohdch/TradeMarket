using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerModel>> GetCustomersByProductIdAsync(int productId)
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();

            var result = customers
                .Where(c => c.Receipts.Any(r => r.ReceiptDetails.Any(rd => rd.ProductId == productId)))
                .Distinct()
                .ToList();

            return _mapper.Map<IEnumerable<CustomerModel>>(result);
        }

        public async Task<IEnumerable<CustomerModel>> GetAllAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllWithDetailsAsync();
            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }

        public async Task<CustomerModel> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdWithDetailsAsync(id);
            return _mapper.Map<CustomerModel>(customer);
        }

        public async Task AddAsync(CustomerModel model)
        {
            ValidateModel(model);

            var customer = _mapper.Map<Customer>(model);

            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(CustomerModel model)
        {
            ValidateModel(model);

            var customer = _mapper.Map<Customer>(model);

            _unitOfWork.PersonRepository.Update(customer.Person);
            _unitOfWork.CustomerRepository.Update(customer);

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.CustomerRepository.DeleteByIdAsync(modelId);
            await _unitOfWork.SaveAsync();
        }

        private void ValidateModel(CustomerModel model)
        {
            if (model == null)
                throw new MarketException("Customer model cannot be null. Invalid customer data.");

            if (string.IsNullOrEmpty(model.Name))
                throw new MarketException("Name cannot be empty.");

            if (string.IsNullOrEmpty(model.Surname))
                throw new MarketException("Surname cannot be null or empty.");

            if (model.BirthDate < DateTime.Now.AddYears(-80) || model.BirthDate > DateTime.Now)
                throw new MarketException("Birth date should be within the last 80 years.");

            if (model.DiscountValue < 0)
                throw new MarketException("Discount cannot be less than 0.");
        }
    }
}
