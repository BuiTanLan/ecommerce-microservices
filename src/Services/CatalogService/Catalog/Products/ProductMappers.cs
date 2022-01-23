using AutoMapper;
using Catalog.Products.Dtos;
using Catalog.Products.Features.CreatingProduct;
using Catalog.Products.Features.CreatingProduct.Requests;
using Catalog.Products.Features.GettingProductsView;
using Catalog.Products.Models;

namespace Catalog.Products;

public class ProductMappers : Profile
{
    public ProductMappers()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(x => x.Depth, opt => opt.MapFrom(x => x.Dimensions.Depth))
            .ForMember(x => x.Height, opt => opt.MapFrom(x => x.Dimensions.Height))
            .ForMember(x => x.Width, opt => opt.MapFrom(x => x.Dimensions.Width))
            .ForMember(x => x.BrandName, opt => opt.MapFrom(x => x.Brand.Name))
            .ForMember(x => x.CategoryName, opt => opt.MapFrom(x => x.Category.Name))
            .ForMember(x => x.SupplierName, opt => opt.MapFrom(x => x.Supplier.Name));

        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<ProductView, ProductViewDto>();

        CreateMap<CreateProductCommand, Product>();

        CreateMap<CreateProductRequest, CreateProductCommand>()
            .ConstructUsing(req => new CreateProductCommand(
                req.Name,
                req.Price,
                req.Stock,
                req.RestockThreshold,
                req.MaxStockThreshold,
                req.Status,
                req.Width,
                req.Height,
                req.Depth,
                req.CategoryId,
                req.SupplierId,
                req.BrandId,
                req.Description,
                req.Images));
    }
}
