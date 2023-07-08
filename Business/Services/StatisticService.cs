using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StatisticService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetCustomersMostPopularProductsAsync(int productCount, int customerId)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var products = receipts
                .Where(r => r.CustomerId == customerId)
                .SelectMany(r => r.ReceiptDetails)
                .GroupBy(rd => rd.ProductId)
                .Select(g => new
                {
                    Product = g.First().Product,
                    Quantity = g.Sum(rd => rd.Quantity)
                })
                .OrderByDescending(p => p.Quantity)
                .Take(productCount)
                .Select(p => p.Product);

            var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products);

            return productsModel.ToList();
        }

        public async Task<decimal> GetIncomeOfCategoryInPeriod(int categoryId, DateTime startDate, DateTime endDate)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            decimal income = receipts
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .SelectMany(r => r.ReceiptDetails)
                .Where(rd => rd.Product.ProductCategoryId == categoryId)
                .Sum(rd => rd.DiscountUnitPrice * rd.Quantity);

            return income;
        }

        public async Task<IEnumerable<ProductModel>> GetMostPopularProductsAsync(int productCount)
        {
            var receiptDetails = await _unitOfWork.ReceiptDetailRepository.GetAllWithDetailsAsync();

            var products = receiptDetails
                .GroupBy(rd => rd.ProductId)
                .Select(g => new
                {
                    Product = g.First().Product,
                    Quantity = g.Sum(rd => rd.Quantity)
                })
                .OrderByDescending(p => p.Quantity)
                .Take(productCount)
                .Select(p => p.Product);

            var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products);

            return productsModel.ToList();
        }

        public async Task<IEnumerable<CustomerActivityModel>> GetMostValuableCustomersAsync(int customerCount, DateTime startDate, DateTime endDate)
        {
            var receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var customersWithSum = receipts
                .Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate)
                .GroupBy(r => r.Customer)
                .Select(g => new
                {
                    Customer = g.Key,
                    ReceiptSum = g.SelectMany(r => r.ReceiptDetails).Sum(rd => rd.DiscountUnitPrice * rd.Quantity)
                })
                .OrderByDescending(c => c.ReceiptSum)
                .Take(customerCount)
                .Select(c => new CustomerActivityModel
                {
                    CustomerId = c.Customer.Id,
                    CustomerName = $"{c.Customer.Person.Name} {c.Customer.Person.Surname}",
                    ReceiptSum = c.ReceiptSum
                });

            return customersWithSum.ToList();
        }
    }
}
