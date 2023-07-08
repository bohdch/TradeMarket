using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Linq;

namespace Business
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Receipt, ReceiptModel>()
                .ForMember(rm => rm.ReceiptDetailsIds, opt => opt.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ReverseMap();

            CreateMap<Product, ProductModel>()
                .ForMember(pm => pm.ReceiptDetailIds, opt => opt.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
                .ForMember(pm => pm.CategoryName, opt => opt.MapFrom(x => x.Category != null ? x.Category.CategoryName : string.Empty))
                .ReverseMap();

            CreateMap<ReceiptDetail, ReceiptDetailModel>()
                .ReverseMap();

            CreateMap<Customer, CustomerModel>()
                .ForMember(cm => cm.Id, opt => opt.MapFrom(x => x.Person.Id))
                .ForMember(cm => cm.Name, opt => opt.MapFrom(x => x.Person.Name))
                .ForMember(cm => cm.Surname, opt => opt.MapFrom(x => x.Person.Surname))
                .ForMember(cm => cm.BirthDate, opt => opt.MapFrom(x => x.Person.BirthDate))
                .ForMember(cm => cm.ReceiptsIds, opt => opt.MapFrom(x => x.Receipts.Select(rd => rd.Id)))
                .ReverseMap();

            CreateMap<ProductCategory, ProductCategoryModel>()
                .ForMember(pm => pm.ProductIds, opt => opt.MapFrom(x => x.Products.Select(p => p.Id)))
                .ReverseMap();
        }
    }
}
