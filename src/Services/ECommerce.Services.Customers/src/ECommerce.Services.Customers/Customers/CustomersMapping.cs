using AutoMapper;
using BuildingBlocks.Core.Domain.ValueObjects;
using ECommerce.Services.Customers.Customers.Dtos;
using ECommerce.Services.Customers.Customers.Models;

namespace ECommerce.Services.Customers.Customers;

public class CustomersMapping : Profile
{
    public CustomersMapping()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id.Value))
            .ForMember(x => x.Country, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address.Country))
            .ForMember(x => x.City, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address.City))
            .ForMember(x => x.DetailAddress, opt => opt.MapFrom(x => x.Address == Address.Null ? "" : x.Address.Detail))
            .ForMember(x => x.Nationality, opt => opt.MapFrom(x => x.Nationality))
            .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(x => x.BirthDate, opt => opt.MapFrom(x => x.BirthDate))
            .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(x => x.PhoneNumber));
    }
}
