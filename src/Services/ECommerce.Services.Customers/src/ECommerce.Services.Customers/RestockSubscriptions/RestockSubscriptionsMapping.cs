using AutoMapper;
using ECommerce.Services.Customers.RestockSubscriptions.Dtos;
using ECommerce.Services.Customers.RestockSubscriptions.Models;
using ECommerce.Services.Customers.RestockSubscriptions.Models.Write;

namespace ECommerce.Services.Customers.RestockSubscriptions;

public class RestockSubscriptionsMapping : Profile
{
    public RestockSubscriptionsMapping()
    {
        CreateMap<RestockSubscription, RestockSubscriptionDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.Value))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.Value))
            .ForMember(x => x.ProductName, opt => opt.MapFrom(x => x.ProductInformation.Name))
            .ForMember(x => x.ProductId, opt => opt.MapFrom(x => x.ProductInformation.Id.Value))
            .ForMember(x => x.CustomerId, opt => opt.MapFrom(x => x.CustomerId.Value));
    }
}
