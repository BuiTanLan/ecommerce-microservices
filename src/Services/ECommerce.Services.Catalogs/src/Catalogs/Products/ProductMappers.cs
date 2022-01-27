using AutoMapper;
using Catalogs.Products.Dtos;
using Catalogs.Products.Features.CreatingProduct;
using Catalogs.Products.Features.CreatingProduct.Requests;
using Catalogs.Products.Features.GettingProductsView;
using Catalogs.Products.Models;

namespace Catalogs.Products;

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

        CreateMap<CreateProduct, Product>();

        CreateMap<CreateProductRequest, CreateProduct>()
            .ConstructUsing(req => new CreateProduct(
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
