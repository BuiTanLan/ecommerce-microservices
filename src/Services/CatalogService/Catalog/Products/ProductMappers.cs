using AutoMapper;
using Catalog.Products.Dtos;
using Catalog.Products.Features.GetProductsView;

namespace Catalog.Products;

public class ProductMappers : Profile
{
    public ProductMappers()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductView, ProductViewDto>();
    }
}
