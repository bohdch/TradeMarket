using System;
using Business.Interfaces;
using Business.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Data.Interfaces;
using Business.Validation;

namespace Business.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReceiptService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReceiptModel>> GetByFilterAsync(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Receipt> receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var receiptsModel = _mapper.Map<IEnumerable<ReceiptModel>>(receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate));

            return receiptsModel;
        }

        public async Task AddProductAsync(int productId, int receiptId, int quantity)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            var receiptDetailWithExceptProduct = receipt.ReceiptDetails?.FirstOrDefault(rd => rd.ProductId == productId);

            if (receiptDetailWithExceptProduct != null)
            {
                receiptDetailWithExceptProduct.Quantity += quantity;

                await _unitOfWork.SaveAsync();

                return;
            }

            Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);

            if (product is null)
            {
                throw new MarketException();
            }

            ReceiptDetail receiptDetail = new ReceiptDetail()
            {
                ProductId = productId,
                Product = product,
                ReceiptId = receiptId,
                Quantity = quantity,
                UnitPrice = product.Price,
                Receipt = receipt,
                DiscountUnitPrice = product.Price - (receipt.Customer.DiscountValue * product.Price / 100m)
            };

            receipt.ReceiptDetails ??= new List<ReceiptDetail>();
            receipt.ReceiptDetails.Add(receiptDetail);

            await _unitOfWork.ReceiptDetailRepository.AddAsync(receiptDetail);

            await _unitOfWork.SaveAsync();
        }

        public async Task CalculateDiscountedPrice(int receiptId)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            foreach (var receiptDetail in receipt.ReceiptDetails)
            {
                receiptDetail.DiscountUnitPrice = receiptDetail.Product.Price - (receipt.Customer.DiscountValue * receiptDetail.Product.Price / 100m);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException("Receipt is null");
            }

            var receiptDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

            if (receiptDetail is null)
            {
                throw new MarketException();
            }

            receiptDetail.Quantity -= quantity;

            if (receiptDetail.Quantity <= 0)
            {
                _unitOfWork.ReceiptDetailRepository.Delete(receiptDetail);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            var receiptDetailsModel = _mapper.Map<IEnumerable<ReceiptDetailModel>>(receipt.ReceiptDetails);

            return receiptDetailsModel;
        }

        public async Task<decimal> ToPayAsync(int receiptId)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            return receipt.ReceiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity);
        }

        public async Task CheckOutAsync(int receiptId)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdAsync(receiptId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            receipt.IsCheckedOut = true;

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            IEnumerable<Receipt> receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var receiptsModel = _mapper.Map<IEnumerable<ReceiptModel>>(receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate));

            return receiptsModel;
        }

        public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
        {
            IEnumerable<Receipt> receipts = await _unitOfWork.ReceiptRepository.GetAllWithDetailsAsync();

            var receiptsModel = _mapper.Map<IEnumerable<ReceiptModel>>(receipts);

            return receiptsModel;
        }

        public async Task<ReceiptModel> GetByIdAsync(int id)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(id);

            var receiptModel = _mapper.Map<ReceiptModel>(receipt);

            return receiptModel;
        }

        public async Task AddAsync(ReceiptModel model)
        {
            ValidateReceiptModel(model);

            Receipt receipt = _mapper.Map<Receipt>(model);

            await _unitOfWork.ReceiptRepository.AddAsync(receipt);

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ReceiptModel model)
        {
            ValidateReceiptModel(model);

            Receipt receipt = _mapper.Map<Receipt>(model);

            _unitOfWork.ReceiptRepository.Update(receipt);

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            Receipt receipt = await _unitOfWork.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

            if (receipt is null)
            {
                throw new MarketException();
            }

            foreach (var rd in receipt.ReceiptDetails.ToArray())
            {
                _unitOfWork.ReceiptDetailRepository.Delete(rd);
            }

            await _unitOfWork.ReceiptRepository.DeleteByIdAsync(modelId);

            await _unitOfWork.SaveAsync();
        }

        private void ValidateReceiptModel(ReceiptModel model)
        {
            if (model == null) throw new MarketException("ReceiptModel cannot be null");
        }
    }
}
