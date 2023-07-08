using System;
using Business.Interfaces;
using Data.Interfaces;
using Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using AutoMapper;
using Business.Validation;
using System.Linq;

namespace Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
        {
            ValidateFilterSearchModel(filterSearch);

            var products = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();

            var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products
                .Where(p => (filterSearch.CategoryId is null || p.ProductCategoryId == filterSearch.CategoryId) &&
                            p.Price >= filterSearch.MinPrice &&
                            p.Price <= filterSearch.MaxPrice));

            return productsModel;
        }

        public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
        {
            var categories = await _unitOfWork.ProductCategoryRepository.GetAllAsync();

            var categoriesModel = _mapper.Map<IEnumerable<ProductCategoryModel>>(categories);

            return categoriesModel;
        }

        public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
        {
            ValidateProductCategoryModel(categoryModel);

            var category = _mapper.Map<ProductCategory>(categoryModel);

            await _unitOfWork.ProductCategoryRepository.AddAsync(category);

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
        {
            ValidateProductCategoryModel(categoryModel);

            var category = _mapper.Map<ProductCategory>(categoryModel);

            _unitOfWork.ProductCategoryRepository.Update(category);

            await _unitOfWork.SaveAsync();
        }

        public async Task RemoveCategoryAsync(int categoryId)
        {
            await _unitOfWork.ProductCategoryRepository.DeleteByIdAsync(categoryId);

            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<ProductModel>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllWithDetailsAsync();

            var productsModel = _mapper.Map<IEnumerable<ProductModel>>(products);

            return productsModel;
        }

        public async Task<ProductModel> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdWithDetailsAsync(id);

            var productModel = _mapper.Map<ProductModel>(product);

            return productModel;
        }

        public async Task AddAsync(ProductModel model)
        {
            ValidateProductModel(model);

            var product = _mapper.Map<Product>(model);

            await _unitOfWork.ProductRepository.AddAsync(product);

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ProductModel model)
        {
            ValidateProductModel(model);

            var product = _mapper.Map<Product>(model);

            _unitOfWork.ProductRepository.Update(product);

            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int modelId)
        {
            await _unitOfWork.ProductRepository.DeleteByIdAsync(modelId);

            await _unitOfWork.SaveAsync();
        }

        private void ValidateProductModel(ProductModel model)
        {
            if (model == null)
                throw new MarketException("ProductModel cannot be null.");

            if (string.IsNullOrEmpty(model.ProductName))
                throw new MarketException("ProductName cannot be null or empty.");

            if (model.Price <= 0)
                throw new MarketException("Price must be greater than 0.");
        }

        private void ValidateProductCategoryModel(ProductCategoryModel model)
        {
            if (model == null)
                throw new MarketException("ProductCategoryModel cannot be null.");

            if (string.IsNullOrEmpty(model.CategoryName))
                throw new MarketException("CategoryName cannot be null or empty.");
        }

        private void ValidateFilterSearchModel(FilterSearchModel model)
        {
            if (model == null)
                throw new MarketException("FilterSearchModel cannot be null.");

            if (model.MinPrice <= 0)
                throw new MarketException("MinPrice must be greater than 0.");

            if (model.MaxPrice <= 0)
                throw new MarketException("MaxPrice must be greater than 0.");

            if (model.MaxPrice < model.MinPrice)
                throw new MarketException("MaxPrice cannot be less than MinPrice.");

            model.MinPrice ??= 0;
            model.MaxPrice ??= int.MaxValue;
        }
    }
}
